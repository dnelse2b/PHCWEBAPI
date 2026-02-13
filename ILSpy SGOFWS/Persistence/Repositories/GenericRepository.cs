using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OPPWS.Extensions;
using SGOFWS.Domains.Interface;
using SGOFWS.Extensions;
using SGOFWS.Persistence.Contexts;

namespace SGOFWS.Persistence.Repositories;

public class GenericRepository : IGenericRepository
{
	private readonly SGOFCTX _SGOFCTX;

	private readonly ConversionExtension conversionExtension = new ConversionExtension();

	public GenericRepository(SGOFCTX SGOFCTX)
	{
		_SGOFCTX = SGOFCTX;
	}

	public void SaveChanges()
	{
		_SGOFCTX.SaveChanges();
	}

	public void BulkOverWrite<T>(List<List<T>> entityLists) where T : class
	{
		IDbContextTransaction transaction = _SGOFCTX.Database.BeginTransaction();
		try
		{
			foreach (List<T> list in entityLists)
			{
				_SGOFCTX.RemoveRange(list);
			}
			_SGOFCTX.SaveChanges();
			foreach (List<T> list2 in entityLists)
			{
				_SGOFCTX.AddRange(list2);
			}
			_SGOFCTX.SaveChanges();
			transaction.Commit();
		}
		catch (Exception ex)
		{
			transaction.Rollback();
			throw new Exception("BULKOVERRITE EXEPTION: " + ex.Message + "INNER GenericRepository Repository: " + ex.InnerException?.ToString() + " Stack BulkOverWrite Repository: " + ex.StackTrace);
		}
	}

	public void BulkUpsertEntity<T>(List<T> entities, List<string> keysToExclude, bool saveChanges) where T : class
	{
		foreach (T entity in entities)
		{
			List<KeyValuePair<string, object>> conditions = new List<KeyValuePair<string, object>>();
			string keyToExclude = keysToExclude.FirstOrDefault();
			if (keyToExclude != null)
			{
				conditions.Add(new KeyValuePair<string, object>(keyToExclude, entity.GetValObjDy(keyToExclude)));
			}
			UpsertEntity(entity, keysToExclude, conditions, saveChanges);
		}
	}

	public void Add<T>(T entity) where T : class
	{
		entity.AssignDefaultEntityValues();
		_SGOFCTX.Add(entity);
	}

	public void Remove<T>(T entity) where T : class
	{
		entity.AssignDefaultEntityValues();
		_SGOFCTX.Remove(entity);
	}

	private static bool IsPropertyInList(PropertyInfo searchProperty, List<PropertyInfo> propertyList)
	{
		return propertyList.Where((PropertyInfo propert) => propert.Name == searchProperty.Name).Any();
	}

	public void UpsertEntity<T>(T entity, List<string> keysToExclude, List<KeyValuePair<string, object>> conditions, bool saveChanges) where T : class
	{
		Type entityType = typeof(T);
		List<PropertyInfo> keysToExcludeProperties = new List<PropertyInfo>();
		List<string> modifiedProperties = entity.GetAssignedProperties();
		foreach (string keyToExclude in keysToExclude)
		{
			PropertyInfo keyToExcludeProperty = entityType.GetProperty(keyToExclude);
			if (keyToExcludeProperty == null)
			{
				throw new InvalidOperationException("Entity does not have a property named '" + keyToExclude + "'.");
			}
			keysToExcludeProperties.Add(keyToExcludeProperty);
		}
		ParameterExpression parameter = Expression.Parameter(entityType, "e");
		Expression combinedCondition = null;
		foreach (KeyValuePair<string, object> condition in conditions)
		{
			PropertyInfo keyProperty = entityType.GetProperty(condition.Key);
			if (keyProperty == null)
			{
				throw new InvalidOperationException($"Entity does not have a property named '{keyProperty}'.");
			}
			PropertyInfo property = entityType.GetProperty(condition.Key);
			object propertyValue = condition.Value;
			MemberExpression propertyExpression = Expression.Property(parameter, property);
			ConstantExpression constant = Expression.Constant(propertyValue);
			BinaryExpression equalExpression = Expression.Equal(propertyExpression, constant);
			combinedCondition = ((combinedCondition != null) ? Expression.AndAlso(combinedCondition, equalExpression) : equalExpression);
		}
		Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(combinedCondition, new ParameterExpression[1] { parameter });
		T existingEntity = _SGOFCTX.Set<T>().Where(lambda).FirstOrDefault();
		if (existingEntity != null)
		{
			PropertyInfo[] properties = entityType.GetProperties();
			foreach (PropertyInfo property2 in properties)
			{
				if (IsPropertyInList(property2, keysToExcludeProperties))
				{
					continue;
				}
				string propertyName = property2.Name.ToString();
				if (!modifiedProperties.Contains(propertyName))
				{
					continue;
				}
				object newValue = property2.GetValue(entity);
				if (newValue != null)
				{
					object finalValue = newValue;
					if (property2.PropertyType == typeof(DateTime))
					{
						finalValue = conversionExtension.ParseToDate(newValue);
					}
					property2.SetValue(existingEntity, finalValue);
				}
			}
			_SGOFCTX.Entry(existingEntity).State = EntityState.Deleted;
			_SGOFCTX.Entry(existingEntity).State = EntityState.Modified;
		}
		else
		{
			entity.AssignDefaultValues();
			_SGOFCTX.Set<T>().Add(entity);
		}
		if (saveChanges)
		{
			_SGOFCTX.SaveChanges();
		}
	}

	public void UpsertEntityBackup<T>(T entity, List<string> keysToExclude, List<KeyValuePair<string, object>> conditions, bool saveChanges) where T : class
	{
		Type entityType = typeof(T);
		List<PropertyInfo> keysToExcludeProperties = new List<PropertyInfo>();
		foreach (string keyToExclude in keysToExclude)
		{
			PropertyInfo keyToExcludeProperty = entityType.GetProperty(keyToExclude);
			if (keyToExcludeProperty == null)
			{
				throw new InvalidOperationException("Entity does not have a property named '" + keyToExclude + "'.");
			}
			keysToExcludeProperties.Add(keyToExcludeProperty);
		}
		ParameterExpression parameter = Expression.Parameter(entityType, "e");
		Expression combinedCondition = null;
		foreach (KeyValuePair<string, object> condition in conditions)
		{
			PropertyInfo keyProperty = entityType.GetProperty(condition.Key);
			if (keyProperty == null)
			{
				throw new InvalidOperationException($"Entity does not have a property named '{keyProperty}'.");
			}
			PropertyInfo property = entityType.GetProperty(condition.Key);
			object propertyValue = condition.Value;
			MemberExpression propertyExpression = Expression.Property(parameter, property);
			ConstantExpression constant = Expression.Constant(propertyValue);
			BinaryExpression equalExpression = Expression.Equal(propertyExpression, constant);
			combinedCondition = ((combinedCondition != null) ? Expression.AndAlso(combinedCondition, equalExpression) : equalExpression);
		}
		Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(combinedCondition, new ParameterExpression[1] { parameter });
		T existingEntity = _SGOFCTX.Set<T>().Where(lambda).FirstOrDefault();
		if (existingEntity != null)
		{
			PropertyInfo[] properties = entityType.GetProperties();
			foreach (PropertyInfo property2 in properties)
			{
				if (IsPropertyInList(property2, keysToExcludeProperties))
				{
					continue;
				}
				object newValue = property2.GetValue(entity);
				if (newValue != null)
				{
					object finalValue = newValue;
					if (property2.PropertyType == typeof(DateTime?))
					{
						finalValue = conversionExtension.ParseToDate(newValue);
					}
					property2.SetValue(existingEntity, finalValue);
				}
			}
			_SGOFCTX.Entry(existingEntity).State = EntityState.Modified;
		}
		else
		{
			_SGOFCTX.Set<T>().Add(entity);
		}
		if (saveChanges)
		{
			_SGOFCTX.SaveChanges();
		}
	}

	public void UpsertEntity2<T>(T entity, string keyToExclude, List<KeyValuePair<string, object>> conditions, bool saveChanges) where T : class
	{
		Type entityType = typeof(T);
		PropertyInfo keyToExcludeProperty = entityType.GetProperty(keyToExclude);
		if (keyToExcludeProperty == null)
		{
			throw new InvalidOperationException($"Entity does not have a property named '{keyToExcludeProperty}'.");
		}
		ParameterExpression parameter = Expression.Parameter(entityType, "e");
		Expression combinedCondition = null;
		foreach (KeyValuePair<string, object> condition in conditions)
		{
			PropertyInfo keyProperty = entityType.GetProperty(condition.Key);
			if (keyProperty == null)
			{
				throw new InvalidOperationException($"Entity does not have a property named '{keyProperty}'.");
			}
			PropertyInfo property = entityType.GetProperty(condition.Key);
			object propertyValue = condition.Value;
			MemberExpression propertyExpression = Expression.Property(parameter, property);
			ConstantExpression constant = Expression.Constant(propertyValue);
			BinaryExpression equalExpression = Expression.Equal(propertyExpression, constant);
			combinedCondition = ((combinedCondition != null) ? Expression.AndAlso(combinedCondition, equalExpression) : equalExpression);
		}
		Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(combinedCondition, new ParameterExpression[1] { parameter });
		T existingEntity = _SGOFCTX.Set<T>().Where(lambda).FirstOrDefault();
		if (existingEntity != null)
		{
			PropertyInfo[] properties = entityType.GetProperties();
			foreach (PropertyInfo property2 in properties)
			{
				if (!(property2 != keyToExcludeProperty))
				{
					continue;
				}
				object newValue = property2.GetValue(entity);
				if (newValue != null)
				{
					object finalValue = newValue;
					if (property2.PropertyType == typeof(DateTime?))
					{
						finalValue = conversionExtension.ParseToDate(newValue);
					}
					property2.SetValue(existingEntity, finalValue);
				}
			}
			_SGOFCTX.Entry(existingEntity).State = EntityState.Modified;
		}
		else
		{
			_SGOFCTX.Set<T>().Add(entity);
		}
		if (saveChanges)
		{
			_SGOFCTX.SaveChanges();
		}
	}

	public void BulkAdd<T>(IEnumerable<T> entityList) where T : class
	{
		_SGOFCTX.AddRange(entityList);
	}

	public void BulkUpdate<T>(IEnumerable<T> entityList) where T : class
	{
		_SGOFCTX.UpdateRange(entityList);
	}

	public void BulkDelete<T>(IEnumerable<T> entityList) where T : class
	{
		_SGOFCTX.RemoveRange(entityList);
	}
}

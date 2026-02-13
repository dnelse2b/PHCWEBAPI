using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using OPPWS.Extensions;

namespace SGOFWS.Extensions;

public static class DbSetExtensions
{
	public static void AddOrUpdate<TEntity>(this DbSet<TEntity> dbSet, TEntity entity) where TEntity : class
	{
		DbContext context = dbSet.GetService<ICurrentDbContext>().Context;
		EntityEntry<TEntity> entry = context.Entry(entity);
		IReadOnlyList<IProperty> primaryKey = context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties;
		object[] keyValues = primaryKey.Select((IProperty p) => entry.Property(p.Name).CurrentValue).ToArray();
		TEntity existingEntity = dbSet.Find(keyValues);
		if (existingEntity == null)
		{
			dbSet.Add(entity);
		}
		else
		{
			context.Entry(existingEntity).CurrentValues.SetValues(entity);
		}
	}

	public static void UpdateOrAddEntities<T>(this DbContext dbContext, List<T> consolidatedRecords, params Expression<Func<T, object>>[] keySelectors) where T : class
	{
		Func<T, object>[] keySelectorsCompiled = keySelectors.Select((Expression<Func<T, object>> selector) => selector.Compile()).ToArray();
		List<string> keyPropertyNames = keySelectors.Select((Expression<Func<T, object>> selector) => ((MemberExpression)selector.Body).Member.Name).ToList();
		Dictionary<string, List<T>> consolidatedRecordsByKey = (from record in consolidatedRecords
			group record by string.Join(":", keySelectorsCompiled.Select((Func<T, object> selector) => selector(record).ToString()))).ToDictionary((IGrouping<string, T> g) => g.Key, (IGrouping<string, T> g) => g.ToList());
		Dictionary<string, List<string>> propertyValues = keyPropertyNames.ToDictionary((string name) => name, (string name) => consolidatedRecords.Select((T record) => keySelectorsCompiled[0](record).ToString()).ToList());
		List<T> existingRecords = dbContext.GetEntitiesByProperties<T>(propertyValues);
		HashSet<string> existingKeys = new HashSet<string>(existingRecords.Select((T record) => string.Join(":", keySelectorsCompiled.Select((Func<T, object> selector) => selector(record).ToString()))));
		List<T> newRecords = consolidatedRecords.Where((T item) => !existingKeys.Contains(string.Join(":", keySelectorsCompiled.Select((Func<T, object> selector) => selector(item).ToString())))).ToList();
		List<T> recordsToUpdate = consolidatedRecords.Where((T item) => existingKeys.Contains(string.Join(":", keySelectorsCompiled.Select((Func<T, object> selector) => selector(item).ToString())))).ToList();
		Type entityType = typeof(T);
		string entityName = entityType.Name;
		if (recordsToUpdate.Count > 0)
		{
			foreach (T record2 in recordsToUpdate)
			{
				record2.AssignDefaultEntityValues();
				dbContext.Entry(record2).State = EntityState.Detached;
			}
			dbContext.UpdateRange(recordsToUpdate);
		}
		if (newRecords.Count > 0)
		{
			foreach (T record3 in newRecords)
			{
				record3.AssignDefaultEntityValues();
			}
			dbContext.AddRange(newRecords);
		}
		dbContext.SaveChanges();
	}

	public static List<T> GetEntitiesByProperties<T>(this DbContext dbContext, Dictionary<string, List<string>> propertyValues) where T : class
	{
		Type entityType = typeof(T);
		ParameterExpression parameter = Expression.Parameter(entityType, "e");
		Expression predicate = null;
		foreach (KeyValuePair<string, List<string>> kvp in propertyValues)
		{
			string propertyName = kvp.Key;
			List<string> values = kvp.Value;
			PropertyInfo property = entityType.GetProperty(propertyName);
			if (property == null)
			{
				throw new ArgumentException($"Property '{propertyName}' not found on entity type '{entityType.Name}'.");
			}
			MemberExpression propertyAccess = Expression.MakeMemberAccess(parameter, property);
			MethodInfo containsMethod = typeof(List<string>).GetMethod("Contains", new Type[1] { typeof(string) });
			ConstantExpression valuesExpression = Expression.Constant(values);
			MethodCallExpression containsExpression = Expression.Call(valuesExpression, containsMethod, propertyAccess);
			predicate = ((predicate != null) ? ((Expression)Expression.AndAlso(predicate, containsExpression)) : ((Expression)containsExpression));
		}
		Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(predicate, new ParameterExpression[1] { parameter });
		return dbContext.Set<T>().AsNoTracking().Where(lambda)
			.ToList();
	}

	public static List<T> GetPagedEntitiesByProperties<T, TKey>(this DbContext dbContext, List<T> entities, Expression<Func<T, TKey>> keySelector, int pageSize = 4000) where T : class
	{
		List<T> existingEntities = new List<T>();
		Func<T, TKey> compiledKeySelector = keySelector.Compile();
		List<string> keys = (from k in entities.Select(compiledKeySelector)
			select k.ToString()).ToList();
		string keyPropertyName = ((MemberExpression)keySelector.Body).Member.Name;
		Dictionary<string, List<string>> keysGroupedByProperty = (from k in keys
			group k by k).ToDictionary((IGrouping<string, string> g) => keyPropertyName, (IGrouping<string, string> g) => g.ToList());
		int totalPages = (int)Math.Ceiling((double)keys.Count / (double)pageSize);
		for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
		{
			List<string> pageItems = keys.Skip(pageIndex * pageSize).Take(pageSize).ToList();
			List<T> results = dbContext.GetEntitiesByProperties<T>(keysGroupedByProperty);
			existingEntities.AddRange(results);
		}
		return existingEntities;
	}
}

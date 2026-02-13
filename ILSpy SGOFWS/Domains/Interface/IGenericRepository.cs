using System.Collections.Generic;

namespace SGOFWS.Domains.Interface;

public interface IGenericRepository
{
	void UpsertEntity<T>(T entity, List<string> keysToExclude, List<KeyValuePair<string, object>> conditions, bool saveChanges) where T : class;

	void Add<T>(T entity) where T : class;

	void Remove<T>(T entity) where T : class;

	void BulkDelete<T>(IEnumerable<T> entityList) where T : class;

	void BulkAdd<T>(IEnumerable<T> entityList) where T : class;

	void BulkUpdate<T>(IEnumerable<T> entityList) where T : class;

	void BulkOverWrite<T>(List<List<T>> entityLists) where T : class;

	void BulkUpsertEntity<T>(List<T> entities, List<string> keyToExclude, bool saveChanges) where T : class;

	void SaveChanges();
}

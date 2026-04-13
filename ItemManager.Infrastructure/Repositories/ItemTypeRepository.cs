using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemManager.Core.Helpers;
using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Repositories
{
    public class ItemTypeRepository : IItemTypeRepository
    {
        private readonly DbHelper _dbHelper;

        // Constructor
        public ItemTypeRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Show All Item Types
        public async Task<IEnumerable<ItemType>> GetAllAsync()
        {
            var itemTypes = new List<ItemType>();

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT ItemTypeID, ItemTypeName, Sort, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate FROM ItemType ORDER BY Sort";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    itemTypes.Add(new ItemType
                    {
                        ItemTypeID = reader.GetInt32(0),
                        ItemTypeName = reader.GetString(1),
                        Sort = reader.GetInt32(2),
                        CreatedBy = reader.GetString(3),
                        CreatedDate = reader.GetDateTime(4),
                        UpdatedBy = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                        UpdatedDate = reader.IsDBNull(6) ? default : reader.GetDateTime(6)
                    });
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occured while fetching ItemTypes.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching ItemTypes.", ex);
            }

            return itemTypes;
        }

        // Show only selected ID find Item Types
        public async Task<ItemType?> GetByIdAsync(int id)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT ItemTypeID, ItemTypeName, Sort, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate FROM ItemType WHERE ItemTypeID = @ItemTypeID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ItemTypeID", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new ItemType
                    {
                        ItemTypeID = reader.GetInt32(0),
                        ItemTypeName = reader.GetString(1),
                        Sort = reader.GetInt32(2),
                        CreatedBy = reader.GetString(3),
                        CreatedDate = reader.GetDateTime(4),
                        UpdatedBy = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                        UpdatedDate = reader.IsDBNull(6) ? default : reader.GetDateTime(6)
                    };
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while fetching ItemType by ID.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching ItemType by ID.", ex);
            }
            
            return null;
        }

        // Insert another row in Table of Item Types
        public async Task AddAsync(ItemType itemType)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"INSERT INTO ItemType (ItemTypeName, Sort, CreatedBy, CreatedDate)
                          VALUES (@ItemTypeName, @Sort, @CreatedBy, @CreatedDate)";

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ItemTypeName", itemType.ItemTypeName);
                command.Parameters.AddWithValue("@Sort", itemType.Sort);
                command.Parameters.AddWithValue("@CreatedBy", itemType.CreatedBy);
                command.Parameters.AddWithValue("@CreatedDate", itemType.CreatedDate);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while adding ItemType.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding ItemType.", ex);
            }
        }

        // Update selected row in Table of Item Types
        public async Task UpdateAsync(ItemType itemType)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"UPDATE ItemType
                          SET ItemTypeName = @ItemTypeName,
                              Sort = @Sort,
                              UpdatedBy = @UpdatedBy,
                              UpdatedDate = @UpdatedDate
                          WHERE ItemTypeID = @ItemTypeID";

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ItemTypeID", itemType.ItemTypeID);
                command.Parameters.AddWithValue("@ItemTypeName", itemType.ItemTypeName);
                command.Parameters.AddWithValue("@Sort", itemType.Sort);
                command.Parameters.AddWithValue("@UpdatedBy", itemType.UpdatedBy);
                command.Parameters.AddWithValue("@UpdatedDate", itemType.UpdatedDate);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while updating ItemType.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating ItemType.", ex);
            }
        }

        // Pagination in ItemType
        public async Task<PagedResult<ItemType>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string search = "",
            string sortColumn = "Sort",
            string sortDirection = "asc")
        {
            var result = new PagedResult<ItemType>
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            try
            {
                // Whitelist sort column and direction
                var allowedColumns = new[] { "ItemTypeName", "Sort" };
                if (!allowedColumns.Contains(sortColumn))
                    sortColumn = "Sort";

                var allowedDirections = new[] { "asc", "desc" };
                if (!allowedDirections.Contains(sortDirection))
                    sortDirection = "asc";

                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                // Query 1 - get total count with search
                var countQuery = @"SELECT COUNT(*) FROM ItemType
                                   WHERE (@Search = '' OR ItemTypeName LIKE @SearchPattern 
                                                    OR CAST(Sort AS VARCHAR) LIKE @SearchPattern)";
                using var countCommand = new SqlCommand(countQuery, connection);
                countCommand.Parameters.AddWithValue("@Search", search);
                countCommand.Parameters.AddWithValue("@SearchPattern", $"%{search}%");
                result.TotalCount = (int)await countCommand.ExecuteScalarAsync();

                // Query 2 - get paged data
                var offset = (pageNumber - 1) * pageSize;
                var dataQuery = $@"SELECT ItemTypeID, ItemTypeName, Sort, CreatedBy,
                                         CreatedDate, UpdatedBy, UpdatedDate
                                  FROM ItemType
                                  WHERE (@Search = '' OR ItemTypeName LIKE @SearchPattern 
                                                   OR CAST(Sort AS VARCHAR) LIKE @SearchPattern)
                                  ORDER BY {sortColumn} {sortDirection}
                                  OFFSET @Offset ROWS
                                  FETCH NEXT @PageSize ROWS ONLY";

                using var dataCommand = new SqlCommand(dataQuery, connection);
                dataCommand.Parameters.AddWithValue("@Search", search);
                dataCommand.Parameters.AddWithValue("@SearchPattern", $"%{search}%");
                dataCommand.Parameters.AddWithValue("@Offset", offset);
                dataCommand.Parameters.AddWithValue("@PageSize", pageSize);

                using var reader = await dataCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Items.Add(new ItemType
                    {
                        ItemTypeID = reader.GetInt32(reader.GetOrdinal("ItemTypeID")),
                        ItemTypeName = reader.GetString(reader.GetOrdinal("ItemTypeName")),
                        Sort = reader.GetInt32(reader.GetOrdinal("Sort")),
                        CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy"))
                                        ? string.Empty
                                        : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                        UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate"))
                                        ? default
                                        : reader.GetDateTime(reader.GetOrdinal("UpdatedDate"))
                    });
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occured while fetching paged ItemTypes.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occured while fetching paged ItemTypes.", ex);
            }

            return result;
        }

    }
}

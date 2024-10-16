﻿using Dapper;
using Domain.Interfaces;
using Domain.Models;
using Infra.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositories.Dapper
{
    public class XPropertyRepository : IXPropertyRepository
    {
        private readonly string _connectionString;
        public XPropertyRepository(ConnectionStrings conns)
        {
            _connectionString = conns.DevLocal;
        }
        public async Task<int> AddProperty(int ClassID, XProperty xproperty)
        {
            try
            {
                using (var _dbConnection = new SqlConnection(_connectionString))
                {
                    _dbConnection.Open();

                    string sql = "INSERT INTO properties VALUES (@ClassID, @PropertyClassID, @Key, @Name);SELECT SCOPE_IDENTITY();";

                    var ids = await _dbConnection.QueryAsync<int>(sql, new
                    {
                        ClassID,
                        xproperty.PropertyClassID,
                        xproperty.Name,
                        xproperty.Key
                    });

                    return ids.First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> RemoveProperty(int PropertyID)
        {
            try
            {
                using (var _dbConnection = new SqlConnection(_connectionString))
                {
                    _dbConnection.Open();

                    string sql = "delete from properties where id = @id";

                    var affectedRows = await _dbConnection.ExecuteAsync(sql, new
                    {
                        id = PropertyID
                    });

                    return affectedRows != 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<XProperty>> GetProperties(int ClassID)
        {
            try
            {
                using (var _dbConnection = new SqlConnection(_connectionString))
                {
                    _dbConnection.Open();

                    var sql = $@"                
                    select 
                        p.*,
                        c.id PClassID,
                        c.*
                    from Properties p
                    inner join classes c on p.PropertyClassID = c.ID
                    where p.ClassID in(
	                    select 
		                    id
	                    from 
		                    Classes c
	                    where 
		                    c.ID = @ClassID
	                    union
	                    select 
		                    parentid 
	                    from 
		                    Ancestries an
	                    where
		                    an.ClassID = @ClassID
                    )
                ";

                    var res = await _dbConnection.QueryAsync<XProperty, XClass, XProperty>(
                        sql,
                        map: (p, c) =>
                        {
                            p.PropertyClass = c;
                            return p;
                        },
                        param: new
                        {
                            ClassID
                        },
                        splitOn: "PClassID"
                    );

                    return res.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<XProperty?> Get(int id)
        {
            try
            {
                using (var _dbConnection = new SqlConnection(_connectionString))
                {
                    _dbConnection.Open();

                    string query = $"SELECT * FROM properties where id = @id";

                    var res = await _dbConnection.QueryAsync<XProperty>(query, new
                    {
                        id
                    });

                    return res.First();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<XProperty>> Get()
        {
            try
            {
                using (var _dbConnection = new SqlConnection(_connectionString))
                {
                    _dbConnection.Open();

                    string query = $"SELECT * FROM properties";

                    var res = await _dbConnection.QueryAsync<XProperty>(query);

                    return res.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<XProperty> Create(XProperty input)
        {
            try
            {
                using (var _dbConnection = new SqlConnection(_connectionString))
                {
                    _dbConnection.Open();

                    string sql = "INSERT INTO properties VALUES (@Key, @Name, @PropertyClassID);SELECT SCOPE_IDENTITY();";

                    var ids = await _dbConnection.QueryAsync<int>(sql, new
                    {
                        input.Name,
                        input.PropertyClassID,
                        input.Key,
                    });

                    input.ID = ids.First();

                    return input;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<XProperty> Update(XProperty input)
        {
            try
            {
                using (var _dbConnection = new SqlConnection(_connectionString))
                {
                    _dbConnection.Open();

                    string sql = "update property set name = @Name, [key] = @Key, PropertyClassID = @PropertyClassID where id = @ID";

                    var affectedRows = await _dbConnection.ExecuteAsync(sql, new
                    {
                        input.ID,
                        input.Name,
                        input.PropertyClassID,
                        input.Key,
                    });

                    return input;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Delete(int ID)
        {
            try
            {
                using (var _dbConnection = new SqlConnection(_connectionString))
                {
                    _dbConnection.Open();

                    string sql = "delete from properties where ID = @id";

                    var affectedRows = await _dbConnection.ExecuteAsync(sql, new
                    {
                        id = ID
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

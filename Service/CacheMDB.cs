using System;
using System.IO;
using System.Data.OleDb;
#if ! UseSystemExtensions
using Mtn.Library.Extensions;
#endif
using Mtn.Library.Utils;

namespace Mtn.Library.Service
{
    public partial class Cache
    {
        #region MDBContainer

        internal class MdbContainer : ContainerTemplate
        {
            #region table Script

            private const string MCreateTableItem = @"
CREATE TABLE TBMtn_CACHE
(
    [Key] Memo primary key,
    [Value] Memo,
    Updated_By_Access_Time FLOAT,
    Absolute_Expiration_Time FLOAT,
    Last_Access_Time DATETIME 
);
";

            private const string MCreateTableItemAudit = @"
CREATE TABLE TBMtn_CACHE_Audit
(
    [Key] Memo,
    [Value] Memo,
    Updated_By_Access_Time FLOAT,
    Absolute_Expiration_Time FLOAT,
    Last_Access_Time DATETIME,
    Created DATETIME
);
";

            private const string MGetValueQuery = @"SELECT [Value] FROM TBMtn_CACHE WHERE [Key] = ?;";
            private const string MContainKey = @"SELECT 1 FROM TBMtn_CACHE WHERE [Key] = ?;";
            private const string MCountQuery = @"SELECT COUNT(0) FROM TBMtn_CACHE;";
            private const string MDeleteStartwith = @"DELETE FROM TBMtn_CACHE WHERE [Key] Like ?;";
            private const string MDeleteKey = @"DELETE FROM TBMtn_CACHE WHERE [Key] = ?;";
            private const string MDeleteClear = @"DELETE FROM TBMtn_CACHE;";
            private const string MAddItemKey = @"INSERT INTO TBMtn_CACHE([Key],[Value],Updated_By_Access_Time,Absolute_Expiration_Time,Last_Access_Time) VALUES(@Key,@Value,@UpAcces,@AbsTime,Now);";
            private const string MAddItemKeyAudit = @"INSERT INTO TBMtn_CACHE_Audit([Key],[Value],Updated_By_Access_Time,Absolute_Expiration_Time,Last_Access_Time,Created) VALUES(@Key,@Value,@UpAcces,@AbsTime,Now,Now);";

            private const string MUpdateItemKey = @"UPDATE TBMtn_CACHE
   SET Last_Access_Time = Now
WHERE [Key] = ?;";

            private const string MUpdateAuditItemKey = @"UPDATE TBMtn_CACHE_Audit
   SET Last_Access_Time = Now
WHERE [Key] = ?;";


            private const string MDeleteExpired = @"DELETE
FROM TBMtn_CACHE
where (Updated_By_Access_Time > 0 AND DATEADD('s',Updated_By_Access_Time,Last_Access_Time) < NOW) OR 
(
Updated_By_Access_Time = 0 AND 
Absolute_Expiration_Time > 0 AND
DATEADD('s',(Absolute_Expiration_Time + {0}),Last_Access_Time) < NOW
);";

            #endregion

            #region atributtes
            private readonly string _mMdbCnnStr;
            private readonly string _mMdbAuditCnnStr;
            private OleDbConnection _mOledbCnn = new System.Data.OleDb.OleDbConnection();
            private OleDbConnection _mOledbCnnAudit = new System.Data.OleDb.OleDbConnection();
            private readonly string _mStoragePath = "";
            private readonly bool _mUseAudit;
            #endregion

            #region Constructor
            /// <summary>
            /// Contructor
            /// </summary>
            /// <param name="storagePath">
            /// <para>Path where the cache items will be saved.</para>
            /// </param>
            /// <param name="keybase">
            /// <para>Name to identify the container, normally the enum representing the container.</para>
            /// </param>
            /// <param name="keepCache">
            /// <para>Indicates whether to keep the cache even after reset the application.</para>
            /// </param>
            /// <param name="audit">
            /// <para>Indicates whether to create a audit file for this container.</para>
            /// </param>
            public MdbContainer(string storagePath,string keybase,bool keepCache ,bool audit)
            {
                if(storagePath.IsNullOrEmptyMtn(true))
                    _mStoragePath = System.IO.Directory.GetCurrentDirectory() + "\\CacheMtn\\";
                else
                    _mStoragePath = storagePath;

                if(_mStoragePath.LastIndexOf("\\") != (_mStoragePath.Length - 1))
                    _mStoragePath += "\\";

                if(!Directory.Exists(_mStoragePath))
                {
                    try
                    {
                        Directory.CreateDirectory(_mStoragePath);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    if(keepCache == false)
                    {
                        try
                        {
                            string filenameToDelete = string.Format("Mtn_CACHE_MDB_{0}*.{1}", (keybase.IsNullOrEmptyMtn(true) ? "" : keybase), "mdb");
                            var filesToDelete = Directory.GetFiles(_mStoragePath, filenameToDelete, SearchOption.TopDirectoryOnly);
                            foreach(string fileToDelete in filesToDelete)
                                File.Delete(fileToDelete);
                        }
                        catch (Exception ex)
                        {
                            Service.Statistics.Add(ex.GetAllMessagesMtn());
                        }
                    }
                }
                string baseGuid = keepCache?"": "_" + Guid.NewGuid().ToString();
                
                string filename = string.Format("{0}Mtn_CACHE_MDB_{1}{2}.{3}", _mStoragePath, (keybase.IsNullOrEmptyMtn(true) ? "" : keybase), baseGuid, "mdb");
                string connStr = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Engine Type=5", filename);
                _mMdbCnnStr = connStr;
                bool fileExist = File.Exists(filename);
                // only create if must not keep cache or the file don't exist, probably because it's first time
                if(keepCache == false || fileExist == false)
                {
                    ADOX.Catalog cat = new ADOX.Catalog();
                    
                    cat.Create(connStr);
                    cat = null;
                }

                _mOledbCnn.ConnectionString = _mMdbCnnStr;
                _mOledbCnn.Open();
                if(keepCache == false || fileExist == false)
                {
                    var cmd = _mOledbCnn.CreateCommand();
                    cmd.CommandText = MCreateTableItem;
                    cmd.ExecuteNonQuery();
                }

                if(audit)
                {
                    string filenameAud = string.Format("{0}Mtn_CACHE_MDB_{1}{2}.{3}", _mStoragePath,  (keybase.IsNullOrEmptyMtn(true) ? "" : keybase) + "_Audit", baseGuid, "mdb");
                    string connStrAud = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Engine Type=5", filenameAud);
                    _mMdbAuditCnnStr = connStrAud;
                    fileExist = File.Exists(filenameAud);
                    if(fileExist == false)
                    {
                        ADOX.Catalog catAud = new ADOX.Catalog();
                        catAud.Create(connStrAud);
                        catAud = null;
                    }

                    _mOledbCnnAudit.ConnectionString = _mMdbAuditCnnStr;
                    _mOledbCnnAudit.Open();

                    if(fileExist == false)
                    {
                        var cmdAudit = _mOledbCnnAudit.CreateCommand();
                        cmdAudit.CommandText = MCreateTableItemAudit;
                        cmdAudit.ExecuteNonQuery();
                    }
                    _mUseAudit = true;
                }

            }
            #endregion 

            #region MDB Commands

            #region GetCommand
            private OleDbCommand GetCommand()
            {
                OleDbCommand retCmd = null;

                lock(_mOledbCnn)
                {
                    if(_mOledbCnn.State != System.Data.ConnectionState.Open)
                    {
                        try
                        {
                            _mOledbCnn.Close();
                            _mOledbCnn = null;
                        }
                        catch
                        {
                            _mOledbCnn = null;
                        }
                        _mOledbCnn = new OleDbConnection(_mMdbCnnStr);
                        _mOledbCnn.Open();
                    }
                    retCmd = _mOledbCnn.CreateCommand();
                }

                return retCmd;
            }
            private OleDbCommand GetAuditCommand()
            {
                OleDbCommand retCmd = null;

                lock(_mOledbCnnAudit)
                {
                    if(_mOledbCnnAudit.State != System.Data.ConnectionState.Open)
                    {
                        try
                        {
                            _mOledbCnnAudit.Close();
                            _mOledbCnnAudit = null;
                        }
                        catch
                        {
                            _mOledbCnnAudit = null;
                        }
                        _mOledbCnnAudit = new OleDbConnection(_mMdbAuditCnnStr);
                        _mOledbCnnAudit.Open();
                    }
                    retCmd = _mOledbCnnAudit.CreateCommand();
                }

                return retCmd;
            }
            #endregion

            #region Contains
            private bool Contains(string key)
            {
                var cmd = GetCommand();
                cmd.CommandText = MContainKey;
                var parm = cmd.CreateParameter();
                parm.DbType = System.Data.DbType.String;
                parm.Value = key;
                cmd.Parameters.Add(parm);
                var ret = cmd.ExecuteScalar();
                if(ret == null)
                    return false;
                else
                    return true;
            }
            #endregion 

            #region Delete 
            private void Delete(string key)
            {
                var cmd = GetCommand();
                cmd.CommandText = MDeleteKey;
                var parm = cmd.CreateParameter();
                parm.DbType = System.Data.DbType.String;
                parm.Value = key;
                cmd.Parameters.Add(parm);
                cmd.ExecuteScalar();                
            }            
            #endregion

            #region Insert
            private void Insert(string key, object value, TimeSpan slideExpiration, TimeSpan absoluteExpirationTime)
            {
                var cmd = GetCommand();
                cmd.CommandText = MAddItemKey; 
                cmd.Parameters.Add(new OleDbParameter("@Key", key));
                cmd.Parameters.Add(new OleDbParameter("@Value", value));
                cmd.Parameters.Add(new OleDbParameter("@UpAcces", slideExpiration.TotalSeconds));
                cmd.Parameters.Add(new OleDbParameter("@AbsTime", absoluteExpirationTime.TotalSeconds));                

                cmd.ExecuteScalar();

                if(_mUseAudit)
                {
                    var cmdAud = GetAuditCommand();
                    cmdAud.CommandText = MAddItemKeyAudit;
                    cmdAud.Parameters.Add(new OleDbParameter("@Key", key));
                    cmdAud.Parameters.Add(new OleDbParameter("@Value", value));
                    cmdAud.Parameters.Add(new OleDbParameter("@UpAcces", slideExpiration.TotalSeconds));
                    cmdAud.Parameters.Add(new OleDbParameter("@AbsTime", absoluteExpirationTime.TotalSeconds));

                    cmdAud.ExecuteScalar();
                }
            }
            #endregion

            #endregion

            #region Add
            /// <summary>
            /// <para>Adds item to cache.</para>
            /// </summary>
            /// <param name="value">
            /// <para>Represents the object to be put in cache.</para>
            /// </param>
            /// <param name="keys">
            /// <para>Represents the cache key.</para>
            /// </param>
            public override void Add(object value, params string[] keys)
            {
                this.Add(value, TimeSpan.Zero, TimeSpan.Zero, keys);
            }
            /// <summary>
            /// <para>Adds item to cache.</para>
            /// </summary>
            /// <param name="value">
            /// <para>Represents the object to be put in cache.</para>
            /// </param>
            /// <param name="slideExpiration">
            /// <para>Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long without calling the item should be removed from the cache.</para>
            /// </param>
            /// <param name="keys">
            /// <para>Represents the cache key.</para>
            /// </param>
            public override void Add(object value, TimeSpan slideExpiration, params string[] keys)
            {
                this.Add(value, slideExpiration, TimeSpan.Zero, keys);
            }
            /// <summary>
            /// <para>Adds item to cache.</para>
            /// </summary>
            /// <param name="value">
            /// <para>Represents the object to be put in cache.</para>
            /// </param>
            /// <param name="slideExpiration">
            /// <para>Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long without calling the item should be removed from the cache.</para>
            /// </param>
            /// <param name="absoluteExpirationTime">
            /// <para>indicates how much time remains in the cache being removed after this period. It will not be used if the parameter slideExpiration is greater than zero .</para>
            /// </param>
            /// <param name="keys">
            /// <para>Represents the cache key.</para>
            /// </param>
            public override void Add(object value, TimeSpan slideExpiration, TimeSpan absoluteExpirationTime, params string[] keys)
            {
                lock(_mOledbCnn)
                {
                    string key = GetByKey(keys);

                    if(this.Contains(key))
                        this.Delete(key);

                    this.Insert(key, value, slideExpiration, absoluteExpirationTime);
                }
            }
            #endregion

            #region Remove
            /// <summary>
            /// <para>Remove item from cache.</para>
            /// </summary>
            /// <param name="keys">
            /// <para>Represents the cache key.</para>
            /// </param>
            public override void Remove(params string[] keys)
            {
                lock(_mOledbCnn)
                {
                    string key = GetByKey(keys);
                    this.Delete(key);
                }
            }
            #endregion

            #region CheckExpired
            /// <summary>
            /// <para>Check all items expired and remove from cache. </para>
            /// </summary>
            public override void CheckExpired()
            {   
                lock(_mOledbCnn)
                {
                    try
                    {
                        var cmd = this.GetCommand();
                        cmd.CommandText = string.Format(MDeleteExpired, Parameter.GetCacheTime);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Service.Statistics.Add(ex.GetAllMessagesMtn());
                    }
                }
            }
            #endregion

            #region ContainsKey
            /// <summary>
            /// <para>Check if cache has item. </para>
            /// </summary>
            /// <param name="keys">
            /// <para>Represents the cache key.</para>
            /// </param>
            /// <returns>
            /// <para>true if contains at least one item in cache for this key.</para>
            /// </returns>
            public override bool ContainsKey(params string[] keys)
            {
                return this.Contains(GetByKey(keys));
            }
            #endregion

            #region Clear
            /// <summary>
            /// <para>Clear cache container</para>
            /// </summary>
            public override void Clear()
            {
                lock(_mOledbCnn)
                {
                    var cmd = this.GetCommand();
                    cmd.CommandText = MDeleteClear;
                    cmd.ExecuteNonQuery();
                }
            }
            /// <summary>
            /// <para>Clear cache with the prefix.</para>
            /// <param name="prefix">
            /// <para>Clear cache container</para>
            /// </param>
            /// </summary>
            public override void Clear(string prefix)
            {
                lock(_mOledbCnn)
                {
                    var cmd = this.GetCommand();
                    cmd.CommandText = MDeleteStartwith;
                    var parm = cmd.CreateParameter();
                    parm.DbType = System.Data.DbType.String;
                    parm.Value = (prefix + "%");
                    cmd.ExecuteNonQuery();
                }
            }
            #endregion

            #region Iterator
            /// <summary>
            /// <para>Iterator, return cache item by key</para>
            /// <param name="keys">
            /// <para>Cache key.</para>
            /// </param>
            /// </summary>
            public override object this[params string[] keys]
            {
                get
                {
                    var cmd = this.GetCommand();
                    cmd.CommandText = MGetValueQuery;
                    var parm = cmd.CreateParameter();
                    parm.DbType = System.Data.DbType.String;
                    parm.Value = GetByKey(keys);
                    cmd.Parameters.Add(parm);
                    object retVal = cmd.ExecuteScalar();
                    if(retVal != null)
                    {
                        var cmdUp = this.GetCommand();
                        cmdUp.CommandText = MUpdateItemKey;
                        var parmUp = cmdUp.CreateParameter();
                        parmUp.DbType = System.Data.DbType.String;
                        parmUp.Value = GetByKey(keys);
                        cmdUp.Parameters.Add(parmUp);
                        cmdUp.ExecuteNonQuery();

                        if(_mUseAudit)
                        {
                            var cmdAud = GetAuditCommand();
                            cmdAud.CommandText = MUpdateAuditItemKey;
                            var parmUpAud = cmdAud.CreateParameter();
                            parmUpAud.DbType = System.Data.DbType.String;
                            parmUpAud.Value = GetByKey(keys);
                            cmdAud.Parameters.Add(parmUpAud);
                            cmdAud.ExecuteNonQuery();
                        }
                    }
                    return retVal;
                }
            }
            #endregion

            #region Count
            /// <summary>
            /// <para>Returns total items in cache container.</para>
            /// </summary>
            public override int Count
            {
                get
                {
                    var cmd = this.GetCommand();
                    cmd.CommandText = MCountQuery;
                    return (int) cmd.ExecuteScalar();
                }
            }
            #endregion
        }



        #endregion
    }
}

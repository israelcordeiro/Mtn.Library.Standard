using System;
using System.Collections.Generic;
using System.Globalization;
using Mtn.Library.Extensions;

namespace Mtn.Library.Entities
{
    /// <summary>    
    /// This class represents a DataPage used to paging data.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Entity class.
    /// </typeparam>
    [Serializable]
    public class DataPage<TEntity>
    {
        /// <summary>    
        /// Total of records.
        /// </summary>
        public Int32 TotalRecords { get; set; }
        /// <summary>    
        /// Total of pages.
        /// </summary>
        public Int32 TotalPages
        {
            get
            {
                try
                {
                    Int32 retValue = this.TotalRecords / this.RecordsPerPage;
                    if (this.TotalRecords % this.RecordsPerPage > 0)
                        retValue += 1;
                    return retValue;
                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                    return 0;
                }

            }
        }
        /// <summary>    
        /// Current page starts from 1.
        /// </summary>
        public Int32 CurrentPage { get; set; }
        /// <summary>    
        /// Total records in current page.
        /// </summary>
        public Int32 TotalRecordsInCurrentPage { get; set; }
        /// <summary>    
        /// Indicates how many records should have (at most) per page.
        /// </summary>
        public Int32 RecordsPerPage { get; set; }
        /// <summary>    
        /// Error number.
        /// </summary>
        public Int32 ErrorNumber { get; set; }
        /// <summary>    
        /// Message in case of error.
        /// </summary>
        public String Message { get; set; }
        /// <summary>    
        /// Indicates whether occurred error .
        /// </summary>
        public Boolean Error { get; set; }
        /// <summary>    
        /// Can be used to indicates if has authorization.
        /// </summary>
        public Boolean Authorized { get; set; }
        /// <summary>    
        /// Can be used to indicates if has permission.
        /// </summary>
        public Boolean Permitted { get; set; }
        /// <summary>    
        /// Indicates wheter has next page .
        /// </summary>
        public Boolean HasNextPage
        {
            get
            {
                return (this.TotalPages - this.CurrentPage > 0);
            }
        }
        /// <summary>    
        /// Indicates wheter has previous page .
        /// </summary>
        public Boolean HasPreviousPage
        {
            get
            {
                return (this.CurrentPage > 1);
            }
        }
        /// <summary>    
        /// Last Acces.
        /// </summary>>
        public DateTime LastAccess { get; set; }
        /// <summary>    
        /// List of Entity, representing  the page of data.
        /// </summary>
        public IList<TEntity> Page { get; set; }

        #region private properties
        private static String _mUnauthorizedMessage;

        private static String UnauthorizedMessage
        {
            get
            {
                if (DataPage<TEntity>._mUnauthorizedMessage == null)
                {
                    string msg = "";
                    #region culture
                    switch (CultureInfo.CurrentCulture.EnglishName)
                    {
                        case "pt-BR":
                        case "pt-PT":
                        case "pt":
                            msg = "Você não está autorizado a receber estes dados.";
                            break;
                        case "ja":
                        case "ja-JP":
                            msg = "あなたはこのデータを受信する権限がありません。";
                            break;
                        case "es":
                        case "es-AR":
                        case "es-BO":
                        case "es-CL":
                        case "es-CO":
                        case "es-CR":
                        case "es-DO":
                        case "es-EC":
                        case "es-SV":
                        case "es-GT":
                        case "es-HN":
                        case "es-MX":
                        case "es-NI":
                        case "es-PA":
                        case "es-PY":
                        case "es-PE":
                        case "es-PR":
                        case "es-ES":
                        case "es-US":
                        case "es-UY":
                        case "es-VE":
                            msg = "Usted no está autorizado para recibir estos datos.";
                            break;
                        case "fr":
                        case "fr-BE":
                        case "fr-CA":
                        case "fr-FR":
                        case "fr-LU":
                        case "fr-MC":
                        case "fr-CH":
                            msg = "Vous n'êtes pas autorisé à recevoir ces données.";
                            break;
                        case "zh-HK":
                        case "zh-MO":
                        case "zh-CN":
                        case "zh-Hans":
                        case "zh-SG":
                        case "zh-TW":
                        case "zh-Hant":
                            msg = "您未被授权接收这些数据。";
                            break;
                        default:
                            msg = "You are not authorized to receive this data.";
                            break;
                    }
                    #endregion
                    DataPage<TEntity>._mUnauthorizedMessage = msg;
                }
                return DataPage<TEntity>._mUnauthorizedMessage;
            }

        }
        private static String _mUnallowedMessage;

        private static String UnallowedMessage
        {
            get
            {
                if (DataPage<TEntity>._mUnallowedMessage == null)
                {
                    string msg = "";
                    #region culture
                    switch (CultureInfo.CurrentCulture.EnglishName)
                    {
                        case "pt-BR":
                        case "pt-PT":
                        case "pt":
                            msg = "Você não possui permissão para receber estes dados.";
                            break;
                        case "ja":
                        case "ja-JP":
                            msg = "あなたは、このようなデータを受信するためのアクセス許可を持っていません。";
                            break;
                        case "es":
                        case "es-AR":
                        case "es-BO":
                        case "es-CL":
                        case "es-CO":
                        case "es-CR":
                        case "es-DO":
                        case "es-EC":
                        case "es-SV":
                        case "es-GT":
                        case "es-HN":
                        case "es-MX":
                        case "es-NI":
                        case "es-PA":
                        case "es-PY":
                        case "es-PE":
                        case "es-PR":
                        case "es-ES":
                        case "es-US":
                        case "es-UY":
                        case "es-VE":
                            msg = "Usted no tiene permiso para recibir estos datos.";
                            break;
                        case "fr":
                        case "fr-BE":
                        case "fr-CA":
                        case "fr-FR":
                        case "fr-LU":
                        case "fr-MC":
                        case "fr-CH":
                            msg = "Vous n'avez pas la permission de recevoir de telles données.";
                            break;
                        case "zh-HK":
                        case "zh-MO":
                        case "zh-CN":
                        case "zh-Hans":
                        case "zh-SG":
                        case "zh-TW":
                        case "zh-Hant":
                            msg = "您没有权限接收这些数据。";
                            break;
                        default:
                            msg = "You do not have permission to receive such data.";
                            break;
                    }
                    #endregion
                    DataPage<TEntity>._mUnallowedMessage = msg;
                }
                return DataPage<TEntity>._mUnallowedMessage;
            }

        }

        #endregion

        /// <summary>
        /// Contructor.
        /// </summary>
        public DataPage()
        {
        }
        /// <summary>    
        /// Contructor.
        /// </summary>
        /// <param name="page">
        /// List of Entity, representing  the page of data.
        /// </param>
        /// <param name="currentPage">
        /// Current page starts from 1.
        /// </param>
        /// <param name="recordsPerPage">
        /// Indicates how many records should have (at most) per page.
        /// </param>
        /// <param name="totalRecords">
        /// <summary>    
        /// Total of records.
        /// </summary>
        /// </param>
        public DataPage(IList<TEntity> page, Int32 currentPage, Int32 recordsPerPage, Int32 totalRecords)
        {
            this.CurrentPage = currentPage;
            this.RecordsPerPage = recordsPerPage;
            this.Page = page;
            this.TotalRecords = totalRecords;
            this.LastAccess = DateTime.Now;
            TotalRecordsInCurrentPage = page.Count;
        }

        #region public auxiliar methods
        /// <summary>    
        /// Returns an empty DataPage indicating there was no authorization.
        /// </summary>
        /// <returns>
        /// Returns an empty DataPage indicating there was no authorization.
        /// </returns>
        public DataPage<TEntity> GetUnauthorizedPage()
        {
            return this.GetUnauthorizedPage(DataPage<TEntity>.UnauthorizedMessage);
        }
        /// <summary>    
        /// Returns an empty DataPage indicating there was no authorization.
        /// </summary>
        /// <param name="message">
        /// Message.
        /// </param>
        /// <returns>
        /// Returns an empty DataPage indicating there was no authorization.
        /// </returns>
        public DataPage<TEntity> GetUnauthorizedPage(String message)
        {
            var ret = new DataPage<TEntity>() { Authorized = false, Permitted = false, ErrorNumber = -1, LastAccess = DateTime.Now, Message = message, CurrentPage = 0, Page = null, RecordsPerPage = 0, TotalRecords = 0, TotalRecordsInCurrentPage = 0 };

            return ret;
        }
        /// <summary>    
        /// Returns an empty DataPage indicating there was no permission.
        /// </summary>
        /// <returns>
        /// Returns an empty DataPage indicating there was no permission.
        /// </returns>
        public DataPage<TEntity> GetUnallowedPage()
        {
            return this.GetUnallowedPage(DataPage<TEntity>.UnallowedMessage);
        }
        /// <summary>    
        /// Returns an empty DataPage indicating there was no permission.
        /// </summary>
        /// <param name="message">
        /// Message.
        /// </param>
        /// <returns>
        /// Returns an empty DataPage indicating there was no permission.
        /// </returns>
        public DataPage<TEntity> GetUnallowedPage(String message)
        {
            var ret = new DataPage<TEntity>() { Authorized = true, Permitted = false, ErrorNumber = -2, LastAccess = DateTime.Now, Message = message, CurrentPage = 0, Page = null, RecordsPerPage = 0, TotalRecords = 0, TotalRecordsInCurrentPage = 0 };

            return ret;
        }
        #endregion
    }
}

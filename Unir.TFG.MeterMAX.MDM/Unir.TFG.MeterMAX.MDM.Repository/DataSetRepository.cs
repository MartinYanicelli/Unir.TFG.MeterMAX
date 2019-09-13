using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Repository.Interfaces;
using DataSet = Unir.TFG.MeterMAX.MDM.Domain.DataSet;

namespace Unir.TFG.MeterMAX.MDM.Repository
{
    public class DataSetRepository : Noanet.XamArch.Infrastructure.Repository<DataSet>, IDataSetRepository
    {
        protected override string GetSelectStatment()
        {
            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.AppendFormat("SELECT {0}.Id, {0}.Name, {0}.Description, dtc.Id, dtc.Name, dtc.Description FROM [{1}] AS {0} ", TableAlias, TableName);
            sqlQuery.AppendFormat("INNER JOIN [DataSets_DataSetComponents] AS dsdsc ON ({0}.Id=dsdsc.DataSetId) INNER JOIN [DataSetComponents] AS dtc ON (dtc.Id=dsdsc.DataSetComponentId)", TableAlias);
            return sqlQuery.ToString();
        }

        public override IList<DataSet> GetAll()
        {
            IDataReader reader = null;

            try
            {
                var result = new Dictionary<int, DataSet>();
                string sqlQuery = GetSelectStatment();
                reader = dbSession.ExecuteReader(sqlQuery, null);
                while (reader.Read())
                {
                    var dataSetId = reader.GetInt32(0);
                    if (!result.ContainsKey(dataSetId))
                    {
                        result.Add(dataSetId, new DataSet() { 
                            Id = dataSetId, 
                            Name = reader.GetString(1),
                            Description = GetDbNullableString(reader[2])
                        }); 
                    }
                    result[dataSetId].DataSetComponents.Add(new DataSetComponent() {
                        Id = reader.GetInt32(3),
                        Name = reader.GetString(4),
                        Description = GetDbNullableString(reader[5])
                    });
                }
                return result.Values.ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, nameof(GetAll));
                throw new Exception("Se produjo un error al intentar recuperar el listado de 'Cronograma de Lectura' disponibles en el Sistema.");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }
            }
        }
    }
}

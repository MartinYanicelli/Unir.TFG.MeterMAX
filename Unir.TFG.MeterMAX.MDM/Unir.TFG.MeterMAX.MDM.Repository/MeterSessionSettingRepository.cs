using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Repository.Interfaces;
using Unir.TFG.MeterMAX.MDM.Domain;
using Noanet.XamArch.Domain;
using System.Data;
using Noanet.XamArch.Infrastructure;
using System.Linq;

namespace Unir.TFG.MeterMAX.MDM.Repository
{
    public abstract class MeterSessionSettingRepository<T> : Noanet.XamArch.Infrastructure.Repository<T>, IMeterSessionSettingRepository<T>
        where T: MeterSessionSetting, new()
    {
        protected abstract string[] GetPropertyNames();

        protected abstract void SetPropertyValue(T entity, string propertyName, object propertyValue);

        protected override string GetSelectStatment()
        {
            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.Append("SELECT mrs.Id, mrs.Name, mrs.InternalReconnectionAttempts, ");
            sqlQuery.Append("ps.Id, ps.UserId, ps.UserName, ps.Password, ccs.Id, ccs.SendAckResponseThershold, ccs.BaudRate, ccs.PacketSize, ccs.NumberOfPackets, ccs.ChannelTrafficTimeout, ccs.InterCharacterTimeout, ccs.ResponseTimeout, ccs.NumberOfRetries, ");
            sqlQuery.AppendFormat("rsc.Id, rsc.MaxReconnectionAttempts, rs.Id, rs.Schedule, {0} ", string.Join(", ", GetPropertyNames()));
            sqlQuery.AppendFormat("FROM [MeterSessionSettings] AS mrs INNER JOIN [{0}] AS {1} ON (mrs.Id={1}.MeterSessionSettingId) INNER JOIN [MeterProtocolSettings] AS ps ON (ps.Id=mrs.ProtocolSettingId) ", TableName, TableAlias);
            sqlQuery.Append("LEFT OUTER JOIN [CommunicationChannelSettings] AS ccs ON (ccs.Id=mrs.CommunicationChannelSettingId) ");
            sqlQuery.Append("LEFT OUTER JOIN [MeterSessionReconnectionSchemas] AS rsc ON (rsc.Id=mrs.ReconnectionSchemaId) ");
            sqlQuery.Append("LEFT OUTER JOIN [ReconnectionSchedules] AS rs ON (rsc.Id=rs.MeterSessionReconnectionSchemaId)");
            return sqlQuery.ToString();
        }

        public override T FindOne(params IEntityPropertyInfo[] args)
        {
            IDataReader dr = null;
            T result = null;
            try
            {
                StringBuilder sqlQuery = new StringBuilder($"{GetSelectStatment()} WHERE ");
                IQueryParameter[] dbParams = new IQueryParameter[args.Length];
                for (int index = 0; index < args.Length; index++)
                {
                    var dbParamName = string.Format("@p{0}", index);
                    sqlQuery.Append($"({args[index].Name}={dbParamName}) AND ");
                    dbParams[index] = new QueryParameter(dbParamName, args[index].Value);
                }
                // remove the last AND
                sqlQuery.Remove(sqlQuery.Length - 5, 5);
                dr = dbSession.ExecuteReader(sqlQuery.ToString(), dbParams);
                while (dr.Read())
                {
                    if (result == null)
                    {
                        result = new T()
                        {
                            Id = dr.GetInt32(0),
                            Name = dr.GetString(1),
                            InternalReconnectionAttempts = dr.GetInt32(2),
                            ProtocolSetting = new MeterProtocolSetting()
                            {
                                Id = dr.GetInt32(3),
                                UserId = dr.GetInt32(4),
                                UserName = dr.GetString(5),
                                Password = dr.GetString(6)
                            },
                        };

                        if (!dr.IsDBNull(7))
                        {
                            result.CommunicationChannelSetting = new CommunicationChannelSetting()
                            {
                                Id = dr.GetInt32(7),
                                SendAckResponseThershold = GetDbNullableInt32(dr[8]),
                                BaudRate = (Domain.Enumerations.BaudRate)dr.GetInt32(9),
                                PacketSize = (Domain.Enumerations.PacketSize) dr.GetInt16(10),
                                NumberOfPackets = dr.GetByte(11),
                                ChannelTrafficTimeout = dr.GetByte(12),
                                InterCharacterTimeout = dr.GetByte(13),
                                ResponseTimeout = dr.GetByte(14),
                                NumberOfRetries = dr.GetByte(15)
                            };
                        }

                        if (!dr.IsDBNull(16))
                        {
                            result.ReconnectionSchema = new MeterSessionReconnectionSchema()
                            {
                                Id = dr.GetInt32(16),
                                MaxReconnectionAttempts = dr.GetInt32(17),
                            };
                        }

                        for (int index = 20; index < dr.FieldCount; index++)
                        {
                            SetPropertyValue(result, dr.GetName(index), dr.GetValue(index));
                        }
                    }

                    if (!dr.IsDBNull(18))
                    {
                        result.ReconnectionSchema.ReconnectionSchedules.Add(new ReconnectionSchedule()
                        {
                            Id = dr.GetInt32(18),
                            Schedule = TimeSpan.FromSeconds(dr.GetInt64(19)),
                            ReconnectionSchema = result.ReconnectionSchema
                        });
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, nameof(FindOne), args);
                throw new Exception("Error al intentar recuperar los parámetros de configuración para la sesión Insitu");
            }
            finally
            {
                if (dr != null)
                {
                    dr.Dispose();
                    dr = null;
                }
            }

        }

        public override IList<T> GetAll()
        {
            IDataReader dr = null;
            var result = new Dictionary<int, T>();
            try
            {
                dr = dbSession.ExecuteReader(GetSelectStatment(), null);
                while (dr.Read())
                {
                    var meterSessionSettingId = dr.GetInt32(0);
                    if (!result.ContainsKey(meterSessionSettingId))
                    {
                        result.Add(meterSessionSettingId, new T()
                        {
                            Id = dr.GetInt32(0),
                            Name = dr.GetString(1),
                            InternalReconnectionAttempts = dr.GetInt32(2),
                            ProtocolSetting = new MeterProtocolSetting()
                            {
                                Id = dr.GetInt32(3),
                                UserId = dr.GetInt32(4),
                                UserName = dr.GetString(5),
                                Password = dr.GetString(6)
                            },
                            
                        });

                        if (!dr.IsDBNull(7))
                        {
                            result[meterSessionSettingId].CommunicationChannelSetting = new CommunicationChannelSetting()
                            {
                                Id = dr.GetInt32(7),
                                SendAckResponseThershold = GetDbNullableInt32(8),
                                BaudRate = (Domain.Enumerations.BaudRate)dr.GetInt32(9),
                                PacketSize = (Domain.Enumerations.PacketSize) dr.GetInt16(10),
                                NumberOfPackets = dr.GetByte(11),
                                ChannelTrafficTimeout = dr.GetByte(12),
                                ResponseTimeout = dr.GetByte(13),
                                NumberOfRetries = dr.GetByte(14)
                            };
                        }
                       
                        if (!dr.IsDBNull(15))
                        {
                            result[meterSessionSettingId].ReconnectionSchema = new MeterSessionReconnectionSchema()
                            {
                                Id = dr.GetInt32(15),
                                MaxReconnectionAttempts = dr.GetInt32(16),
                            };
                        }

                        for (int index = 19; index < dr.FieldCount; index++)
                        {
                            SetPropertyValue(result[meterSessionSettingId], dr.GetName(index), dr.GetValue(index));
                        }
                    }

                    if (!dr.IsDBNull(17))
                    {
                        result[meterSessionSettingId].ReconnectionSchema.ReconnectionSchedules.Add(new ReconnectionSchedule()
                        {
                            Id = dr.GetInt32(17),
                            Schedule = TimeSpan.FromTicks(dr.GetInt64(18)),
                            ReconnectionSchema = result[meterSessionSettingId].ReconnectionSchema
                        });
                    }
                }
                return result.Values.ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, nameof(GetAll));
                throw new Exception("Error al intentar recuperar el listado de todas las configuraciones disponibles en el Sistema para las sesiones del tipo Insitu");
            }
            finally
            {
                if (dr != null)
                {
                    dr.Dispose();
                    dr = null;
                }
            }
        }
    }
}

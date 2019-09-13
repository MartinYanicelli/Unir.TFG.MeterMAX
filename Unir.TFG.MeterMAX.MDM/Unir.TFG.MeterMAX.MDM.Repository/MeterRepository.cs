using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Repository.Interfaces;

namespace Unir.TFG.MeterMAX.MDM.Repository
{
    public class MeterRepository : Noanet.XamArch.Infrastructure.Repository<Meter>, IMeterRepository
    {
        protected override string GetSelectStatment()
        {
            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.AppendFormat("SELECT {0}.Id, {0}.Model, {0}.SerialNumber, mm.Id, mm.Name, es.Id, es.Code, a.Id, s.Id, s.Name, s.Number, s.Floor, ", TableAlias);
            sqlQuery.Append("pc.Id, pc.Code, city.Id, city.Name, st.Id, st.Name, ct.Id, ct.Name, c.Id, c.IdentificationNumber, c.Name, ");
            sqlQuery.Append("esg.Id, esg.Name, g.Id, g.Altitude, g.Latitude, g.Longitude, ");
            sqlQuery.Append("rd.Id, rd.Model, rd.SerialNumber, rd.Ip, rd.PortNumber, v.Id, v.Name, ");
            sqlQuery.AppendFormat("sc.Id, sc.SerialNumber, phcom.Id, phcom.Name, scs.Id, scs.Name FROM [{0}] AS {1} ", TableName, TableAlias);
            sqlQuery.Append("INNER JOIN  [MeterManufacturers] AS mm ON (mm.Id=m.MeterManufacturerId) ");
            sqlQuery.AppendFormat("INNER JOIN [EnergySupplies] AS es ON (es.Id={0}.EnergySupplyId) ", TableAlias);
            sqlQuery.Append("INNER JOIN [Addresses] AS a ON (a.Id=es.AddressId) INNER JOIN [Streets] AS s ON (s.Id=a.StreetId) ");
            sqlQuery.Append("INNER JOIN [PostalCodes] AS pc ON (pc.Id=a.PostalCodeId) INNER JOIN [Cities] AS city ON (city.Id=pc.CityId) ");
            sqlQuery.Append("INNER JOIN [States] AS st ON (st.Id=city.StateId) INNER JOIN [Countries] AS ct ON (ct.Id=st.CountryId) ");
            sqlQuery.Append("INNER JOIN [Customers] AS c ON (c.Id=es.CustomerId) ");
            sqlQuery.Append("LEFT OUTER JOIN [EnergySupplyGroups] AS esg ON (esg.Id=es.GroupId) ");
            sqlQuery.Append("LEFT OUTER JOIN [GeoCoordinates] AS g ON (g.Id=es.GeoCoordinateId) ");
            sqlQuery.Append("LEFT OUTER JOIN [RemoteDevices] AS rd ON (rd.Id=m.RemoteDeviceId) ");
            sqlQuery.Append("LEFT OUTER JOIN [Vendors] AS v ON (v.Id=rd.VendorId) ");
            sqlQuery.Append("LEFT OUTER JOIN [SIMCards] AS sc ON (sc.Id=rd.SIMCardId) LEFT OUTER JOIN [PhoneCompanies] as phcom ON (phcom.Id=sc.PhoneCompanyId) ");
            sqlQuery.Append("LEFT OUTER JOIN [SIMCardServices] AS scs ON (scs.Id=sc.ServiceId)");
            return sqlQuery.ToString();
        }

        protected override Meter BuildNewEntity(object[] data, [CallerMemberName] string callerMethodName = null)
        {
            var meter = new Meter() {
                Id = Convert.ToInt32(data[0]),
                Model = data[1].ToString(),
                SerialNumber = Convert.ToInt64(data[2]),
                MeterManufacturer = new MeterManufacturer() {
                    Id = Convert.ToInt32(data[3]),
                    Name = data[4].ToString()
                },
                EnergySupply = new EnergySupply() {
                    Id = Convert.ToInt32(data[5]),
                    Code = data[6].ToString(),
                    Address = new Address() {
                        Id = Convert.ToInt32(data[7]),
                        Street = new Street() {
                            Id = Convert.ToInt32(data[8]),
                            Name = data[9].ToString(),
                            Number = Convert.ToInt32(data[10]),
                            Floor = !IsDbNull(data[11]) ? data[11].ToString() : null
                        },
                        PostalCode = new PostalCode() {
                            Id = Convert.ToInt32(data[12]),
                            Code = data[13].ToString(),
                            City = new City() {
                                Id = Convert.ToInt32(data[14]),
                                Name = data[15].ToString(),
                                State = new State() {
                                    Id = Convert.ToInt32(data[16]),
                                    Name = data[17].ToString(),
                                    Country = new Country() {
                                        Id = Convert.ToInt32(data[18]),
                                         Name = data[19].ToString()
                                    }
                                }
                             }
                        }
                    },
                    Customer = new Customer() {
                        Id = Convert.ToInt32(data[20]),
                        IdentificationNumber = data[21].ToString(),
                        Name = data[22].ToString()
                    }
                }
            };

            if (!IsDbNull(data[23]))
            {
                meter.EnergySupply.Group = new EnergySupplyGroup()
                {
                    Id = Convert.ToInt32(data[23]), Name = data[24].ToString()
                };
            }

            if (!IsDbNull(data[25]))
            {
                meter.EnergySupply.GeoCordinate = new GeoCordinate() {
                    Id = Convert.ToInt32(data[25]),
                    Altitude = !IsDbNull(data[26]) ? Convert.ToDouble(data[26]) : (double?)null,
                    Latitude = Convert.ToDouble(data[27]),
                    Longitude = Convert.ToDouble(data[28])
                };
            }

            if (!IsDbNull(data[29]))
            {
                meter.RemoteDevice = new RemoteDevice()
                {
                    Id = Convert.ToInt32(data[29]),
                    Model = data[30].ToString(),
                    SerialNumber = data[31].ToString(),
                    Ip = data[32].ToString(),
                    PortNumber = Convert.ToInt32(data[33]),
                    Vendor = new Vendor()
                    {
                        Id = Convert.ToInt32(data[34]),
                        Name = data[35].ToString()
                    },
                    SIMCard = new SIMCard()
                    {
                        Id = Convert.ToInt32(data[36]),
                        SerialNumber = data[37].ToString(),
                        PhoneCompany = new PhoneCompany()
                        {
                            Id = Convert.ToInt32(data[38]),
                            Name = data[39].ToString()
                        },
                        Service = new SIMCardService()
                        {
                            Id = Convert.ToInt32(data[40]),
                            Name = data[41].ToString()
                        }
                    }
                };
            }
            return meter;
        }

        public IList<Meter> GetAllWithRemoteDevice()
        {
            throw new NotImplementedException();
        }
    }
}

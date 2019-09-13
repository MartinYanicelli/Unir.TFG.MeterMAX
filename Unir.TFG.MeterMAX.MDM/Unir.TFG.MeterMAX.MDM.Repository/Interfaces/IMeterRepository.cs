using System;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.MDM.Domain;

namespace Unir.TFG.MeterMAX.MDM.Repository.Interfaces
{
    public interface IMeterRepository : Noanet.XamArch.Domain.PersistenceSupport.IRepository<Meter>
    {
        IList<Meter> GetAllWithRemoteDevice();
    }
}

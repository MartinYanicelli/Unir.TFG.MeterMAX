using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Utils;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Enumerations;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public abstract class BillingRegisterDataTable : TableParser
    {
        public PrimaryMeteringInformation PrimaryMeteringInformation { get; set; }
        public DataSelectionTable DataSelectionTable { get; set; }

        public readonly BillingDataBlock TotalDataBlock;
        public readonly IList<Tier> Tiers;

        protected BillingRegisterDataTable() 
            : this(null)
        {
           
        }

        protected BillingRegisterDataTable(DataSelectionTable dataSelectionTable) 
        {
            DataSelectionTable = dataSelectionTable;
            
            Tiers = new List<Tier>();
            TotalDataBlock = new BillingDataBlock("TotalDataBlock");
        }

        protected override void OnParse(byte[] table)
        {
            if (DataSelectionTable == null)
                throw new InvalidOperationException("La propiedad DataSelectionTable no pueden ser nulas.");

            //Empieza en el segundo byte
            OnParse(table, 1);
        }

        protected void OnParse(byte[] table, int startOffset)
        {
            int initial = startOffset; 
            
            for (int index = 0; index < DataSelectionTable.ActualRegisterTable.NumberOfSummations; index++)
            {
                byte[] summationArray = new byte[6];
                int offset = initial + index * 6;//Cada dato tiene 6 bytes
                Array.Copy(table, offset, summationArray, 0, 6);
                Array.Reverse(summationArray);
                JIBitArray jb = new JIBitArray(summationArray);
                long summation = 0;
                if (jb.Get(0) == true) //Es negativo hay que hacer complemento a 2
                {
                    jb = jb.Not();
                    summation = jb.GetLong()[0];
                    summation = summation + 1;
                    summation = -1 * summation;
                }
                else
                {
                    summation = jb.GetLong()[0];
                }
                
                Source registerSource = DataSelectionTable.SummationSelect[index];
                BillingRegister summationRegister = new BillingRegister() { 
                    Value = summation * (decimal) Math.Pow(10, registerSource.ScaleFactor),
                    UOM = Helper.ConvertUOMCodeToString(registerSource.UOMCode) + "h",
                    Direction = registerSource.Flow.ToString()
                };
                TotalDataBlock.Summations.Add(summationRegister);
            }

            initial = initial + (DataSelectionTable.ActualRegisterTable.NumberOfSummations * 6);
            for (int index = 0; index < DataSelectionTable.ActualRegisterTable.NumberOfDemmands; index++)
            {
                int offset = initial + index * (5 + 6 + 5); //Cada dato tiene 5+6+5 bytes
                byte[] dateArray = new byte[5];
                byte[] cumulativeArray = new byte[6];
                byte[] maxArray = new byte[5];
                Array.Copy(table, offset, dateArray, 0, 5);
                Array.Copy(table, offset + 5, cumulativeArray, 0, 6);
                Array.Reverse(cumulativeArray);
                JIBitArray jb = new JIBitArray(cumulativeArray);
                long cumulative = 0;
                if (jb.Get(0) == true) //Es negativo hay que hacer complemento a 2
                {
                    jb = jb.Not();
                    cumulative = jb.GetLong()[0];
                    cumulative = cumulative + 1;
                    cumulative = -1 * cumulative;
                }
                else
                {
                    cumulative = jb.GetLong()[0];
                }
                Array.Copy(table, offset + 5 + 6, maxArray, 0, 5);
                Array.Reverse(maxArray);
                jb = new JIBitArray(maxArray);
                long maximum = 0;
                if (jb.Get(0) == true) //Es negativo hay que hacer complemento a 2
                {
                    jb = jb.Not();
                    maximum = jb.GetLong()[0];
                    maximum = maximum + 1;
                    maximum = -1 * maximum;
                }
                else
                {
                    maximum = jb.GetLong()[0];
                }

                var demandSource = DataSelectionTable.DemandSelect[index];
                string uom = Helper.ConvertUOMCodeToString(demandSource.UOMCode);
                Demand demand = new Demand()
                {
                    Max = new BillingRegister()
                    {
                        Value = maximum * (decimal) Math.Pow(10, demandSource.ScaleFactor),
                        UOM = uom,
                        Direction = demandSource.Flow.ToString()
                    },
                    Cumulative = new BillingRegister() {
                        Value = cumulative * (decimal) Math.Pow(10, demandSource.ScaleFactor),
                        UOM = uom,
                        Direction = demandSource.Flow.ToString()
                    },
                    Date = Helper.ConvertToDateTime(dateArray)
                };

                TotalDataBlock.Demands.Add(demand);
            }

            initial = initial + DataSelectionTable.ActualRegisterTable.NumberOfDemmands * (5 + 6 + 5);
            for (int index = 0; index < DataSelectionTable.ActualRegisterTable.NumberOfCoincidentValues; index++)
            {
                byte[] coincidentArray = new byte[5];
                int offset = initial + index * 5;//Cada dato tiene 5 bytes
                Array.Copy(table, offset, coincidentArray, 0, 5);
                Array.Reverse(coincidentArray);
                JIBitArray jb = new JIBitArray(coincidentArray);
                long coincident = 0;
                if (jb.Get(0) == true) //Es negativo hay que hacer complemento a 2
                {
                    jb = jb.Not();
                    coincident = jb.GetLong()[0];
                    coincident = coincident + 1;
                    coincident = -1 * coincident;
                }
                else
                {
                    coincident = jb.GetLong()[0];
                }
                Source registerSource = DataSelectionTable.CoincidentSelect[index];
                BillingRegister coincidentRegister = new BillingRegister() { 
                    Value = coincident * (decimal) Math.Pow(10, registerSource.ScaleFactor),
                    UOM = Helper.ConvertUOMCodeToString(registerSource.UOMCode),
                    Direction = registerSource.Flow.ToString()
                };
                TotalDataBlock.Coincidents.Add(coincidentRegister);
            }

            initial = initial + DataSelectionTable.ActualRegisterTable.NumberOfCoincidentValues * 5;
            
            //Empiezo con las capas o fases
            for (int k = 0; k < DataSelectionTable.ActualRegisterTable.NumberOfTiers; k++)
            {
                Tier tier = new Tier(k);
                for (int index = 0; index < DataSelectionTable.ActualRegisterTable.NumberOfSummations; index++)
                {
                    byte[] summationArray = new byte[6];
                    int offset = initial + index * 6;//Cada dato tiene 6 bytes
                    Array.Copy(table, offset, summationArray, 0, 6);
                    Array.Reverse(summationArray);
                    JIBitArray jb = new JIBitArray(summationArray);
                    long summation = 0;
                    if (jb.Get(0) == true) //Es negativo hay que hacer complemento a 2
                    {
                        jb = jb.Not();
                        summation = jb.GetLong()[0];
                        summation = summation + 1;
                        summation = -1 * summation;
                    }
                    else
                    {
                        summation = jb.GetLong()[0];
                    }
                    
                    Source registerSource = DataSelectionTable.SummationSelect[index];
                    BillingRegister summationRegister = new BillingRegister()
                    {
                        Value = summation * (decimal) Math.Pow(10, registerSource.ScaleFactor),
                        UOM = Helper.ConvertUOMCodeToString(registerSource.UOMCode) + "h",
                        Direction = registerSource.Flow.ToString()
                    };


                    tier.Summations.Add(summationRegister);
                }

                initial = initial + DataSelectionTable.ActualRegisterTable.NumberOfSummations * 6;
                for (int index = 0; index < DataSelectionTable.ActualRegisterTable.NumberOfDemmands; index++)
                {
                    int offset = initial + index * (5 + 6 + 5);//Cada dato tiene 5+6+5 bytes
                    byte[] dateArray = new byte[5];
                    byte[] cumulativeArray = new byte[6];
                    byte[] maxArray = new byte[5];
                    Array.Copy(table, offset, dateArray, 0, 5);
                    Array.Copy(table, offset + 5, cumulativeArray, 0, 6);
                    Array.Reverse(cumulativeArray);
                    JIBitArray jb = new JIBitArray(cumulativeArray);
                    long cumulative = 0;
                    if (jb.Get(0) == true) //Es negativo hay que hacer complemento a 2
                    {
                        jb = jb.Not();
                        cumulative = jb.GetLong()[0];
                        cumulative = cumulative + 1;
                        cumulative = -1 * cumulative;
                    }
                    else
                    {
                        cumulative = jb.GetLong()[0];
                    }
                    Array.Copy(table, offset + 5 + 6, maxArray, 0, 5);
                    Array.Reverse(maxArray);
                    jb = new JIBitArray(maxArray);
                    long maximum = 0;
                    if (jb.Get(0) == true) //Es negativo hay que hacer complemento a 2
                    {
                        jb = jb.Not();
                        maximum = jb.GetLong()[0];
                        maximum = maximum + 1;
                        maximum = -1 * maximum;
                    }
                    else
                    {
                        maximum = jb.GetLong()[0];
                    }
                    
                    var demandSource = DataSelectionTable.DemandSelect[index];
                    string uom = Helper.ConvertUOMCodeToString(demandSource.UOMCode);
                    Demand demand = new Demand()
                    {
                        Max = new BillingRegister()
                        {
                            Value = maximum * (decimal) Math.Pow(10, demandSource.ScaleFactor),
                            UOM = uom,
                            Direction = demandSource.Flow.ToString()
                        },
                        Cumulative = new BillingRegister()
                        {
                            Value = cumulative * (decimal) Math.Pow(10, demandSource.ScaleFactor),
                            UOM = uom,
                            Direction = demandSource.Flow.ToString()
                        },
                        Date = Helper.ConvertToDateTime(dateArray)
                    };

                    tier.Demands.Add(demand);
                }

                initial = initial + DataSelectionTable.ActualRegisterTable.NumberOfDemmands * (5 + 6 + 5);
                for (int index = 0; index < DataSelectionTable.ActualRegisterTable.NumberOfCoincidentValues; index++)
                {
                    byte[] coincidentArray = new byte[5];
                    int offset = initial + index * 5;//Cada dato tiene 5 bytes
                    Array.Copy(table, offset, coincidentArray, 0, 5);
                    Array.Reverse(coincidentArray);
                    JIBitArray jb = new JIBitArray(coincidentArray);
                    long coincident = 0;
                    if (jb.Get(0) == true) //Es negativo hay que hacer complemento a 2
                    {
                        jb = jb.Not();
                        coincident = jb.GetLong()[0];
                        coincident = coincident + 1;
                        coincident = -1 * coincident;
                    }
                    else
                    {
                        coincident = jb.GetLong()[0];
                    }
                    Source registerSource = DataSelectionTable.CoincidentSelect[index];
                    BillingRegister coincidentRegister = new BillingRegister()
                    {
                        Value = coincident * (decimal) Math.Pow(10, registerSource.ScaleFactor),
                        UOM = Helper.ConvertUOMCodeToString(registerSource.UOMCode),
                        Direction = registerSource.Flow.ToString()
                    };
                    tier.Coincidents.Add(coincidentRegister);
                }
                initial = initial + DataSelectionTable.ActualRegisterTable.NumberOfCoincidentValues * 5;
                Tiers.Add(tier);
            }
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine(TotalDataBlock.Name);

            for (int i = 0; i < TotalDataBlock.Summations.Count; i++)
            {
                result.AppendFormat("Summation{0}: {1}; ", i, TotalDataBlock.Summations[i]);
                result.AppendLine();
            }
            result.AppendLine();
            for (int i = 0; i < TotalDataBlock.Demands.Count; i++)
            {
                Demand demand = (Demand)TotalDataBlock.Demands[i];
                result.AppendFormat("Demand{0}: Date: {1} - Cumulative: {2} - Max: {3}; ", i, demand.Date, demand.Cumulative, demand.Max);
                result.AppendLine();
            }
            result.AppendLine();
            for (int i = 0; i < TotalDataBlock.Coincidents.Count; i++)
            {
                result.AppendFormat("Coincidents{0}: {1}; ", i, TotalDataBlock.Coincidents[i]);
                result.AppendLine();
            }
            result.AppendLine();
            result.AppendLine("---Tiers---");
            //Capas 
            for (int k = 0; k < DataSelectionTable.ActualRegisterTable.NumberOfTiers; k++)
            {
                Tier tier = (Tier)Tiers[k];
                result.Append(tier.Name);
                result.AppendLine();
                for (int i = 0; i < tier.Summations.Count; i++)
                {
                    result.AppendFormat("Summation{0}: {1}; ", i, tier.Summations[i]);
                    result.AppendLine();
                }
                result.AppendLine();
                for (int i = 0; i < tier.Demands.Count; i++)
                {
                    Demand demand = (Demand)(tier.Demands[i]);
                    result.AppendFormat("Demand{0}: Date: {1} - Cummulative: {2} - Max: {3}; ", i, demand.Date, demand.Cumulative, demand.Max);
                    result.AppendLine();
                }
                result.AppendLine();
                for (int i = 0; i < tier.Coincidents.Count; i++)
                {
                    result.AppendFormat("Coincidents{0}: {1}; ", i, tier.Coincidents[i]);
                    result.AppendLine();
                }
                result.AppendLine();
            }

            return result.ToString();
        }
    }
}

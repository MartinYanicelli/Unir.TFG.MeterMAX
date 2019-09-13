using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Utils;
using Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21.Core;

namespace Unir.TFG.MeterMAX.Interpreter.ANSI.C12_21
{
    public class EventLogDataTable : TableParser
    {
        public int NumberOfValidEntries { get; private set; }
        public IList<Event> Events { get; private set;}
        
        public EventLogDataTable()
        {
            Events = new List<Event>();
        }

        protected override void OnParse(byte[] table)
        {
            int inicial = 1;
            byte[] numberOfValidEntriesArray = new byte[2];
            Array.Copy(table, inicial, numberOfValidEntriesArray, 0, 2);
            Array.Reverse(numberOfValidEntriesArray);
            JIBitArray jb = new JIBitArray(numberOfValidEntriesArray);
            NumberOfValidEntries = jb.GetInt()[0];

            inicial = inicial + 2 + 2 + 4 + 2;

            int offset = inicial;
            for (int i = 0; i < NumberOfValidEntries; i++)
            {
                int year = (int)table[offset];
                offset++;
                int month = (int)table[offset];
                offset++;
                int day = (int)table[offset];
                offset++;
                int hour = (int)table[offset];
                offset++;
                int minutes = (int)table[offset];
                offset++;
                int seconds = (int)table[offset];
                offset++;

                Event ev = new Event
                {
                    DateTime = new DateTime(year, month, day, hour, minutes, seconds)
                };

                byte[] sequenceArray = new byte[2];
                Array.Copy(table, offset, numberOfValidEntriesArray, 0, 2);
                Array.Reverse(sequenceArray);
                JIBitArray jb1 = new JIBitArray(sequenceArray);
                ev.SequenceNumber = jb1.GetInt()[0];
                offset = offset + 2;

                byte[] userIdArray = new byte[2];
                Array.Copy(table, offset, userIdArray, 0, 2);
                Array.Reverse(userIdArray);
                JIBitArray jb2 = new JIBitArray(userIdArray);
                ev.UserId = jb2.GetInt()[0];
                offset = offset + 2;

                byte[] eventIdArray = new byte[2];
                Array.Copy(table, offset, eventIdArray, 0, 2);
                Array.Reverse(eventIdArray);
                JIBitArray jb3 = new JIBitArray(eventIdArray);
                ev.EventId = jb3.SubJIBitArray(5, 11).GetInt()[0];
                offset = offset + 2;

                switch (ev.EventId)
                {
                    case 1:
                        ev.Descripcion = "Power down";
                        break;
                    case 2:
                        ev.Descripcion = "Power up";
                        break;
                    case 3:
                        ev.Descripcion = "Time changed (time stamp = old time)";
                        break;
                    case 4:
                        ev.Descripcion = "Time changed (time stamp = new time)";
                        break;
                    case 18:
                        ev.Descripcion = "Event log cleared";
                        break;
                    case 20:
                        ev.Descripcion = "Demand reset";
                        break;
                    case 32:
                        ev.Descripcion = "Test mode start";
                        break;
                    case 33:
                        ev.Descripcion = "Test mode exit";
                        break;
                }
                Events.Add(ev);
            }

        }
        
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("Number of Valid Entries: {0}", NumberOfValidEntries);
            result.AppendLine();
            for (int i = 0; i < Events.Count; i++)
            {
                Event ev = (Event)Events[i];
                result.AppendFormat("DateTime: {0} - Sequence: {1} - UserId: {2} - EventId: {3} - Description: {4}", ev.DateTime, ev.SequenceNumber, ev.UserId, ev.EventId, ev.Descripcion);
                result.AppendLine();
            }
            return result.ToString();
        }
    }
}

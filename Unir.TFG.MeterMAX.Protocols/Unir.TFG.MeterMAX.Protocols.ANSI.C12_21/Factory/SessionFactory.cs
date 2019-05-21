using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Enumerations;
using Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Services;
using System.Reflection;

namespace Unir.TFG.MeterMAX.Protocols.ANSI.C12_21.Factory
{
    public class SessionFactory
    {
        public static ISession Creator(SessionType type, params object[] args)
        {
            if ((args == null) ||
                ((type == SessionType.Optical) && ((args.Length != 4) && (args.Length != 5) && (args.Length != 8) && (args.Length != 9) && (args.Length != 10))) ||
                ((type == SessionType.Remote) && ((args.Length != 7) && (args.Length != 8) && (args.Length != 9))))
                throw new ArgumentException();

            Type sessionType = (type == SessionType.Optical) ? typeof(OnPlaceSession) : typeof(RemoteSession);
            ConstructorInfo sessionConstructor = null;
            Type[] constructorParamsTypes = null;
            object[] constructorParamsValues = null;

            if (type == SessionType.Optical)
            {
                if (args.Length == 4)
                {
                    constructorParamsTypes = new Type[] { typeof(string), typeof(int), typeof(string), typeof(string) };
                    constructorParamsValues = new object[] { Convert.ToString(args[0]), Convert.ToInt32(args[1]), Convert.ToString(args[2]), Convert.ToString(args[3]) };
                }
                else if (args.Length == 5)
                {
                    constructorParamsTypes = new Type[] { typeof(string), typeof(int), typeof(string), typeof(string), typeof(bool?) };
                    constructorParamsValues = new object[] { Convert.ToString(args[0]), Convert.ToInt32(args[1]), Convert.ToString(args[2]), Convert.ToString(args[3]), (bool?)args[4] };
                }
                else if (args.Length == 6)
                {
                    constructorParamsTypes = new Type[] { typeof(string), typeof(int), typeof(string), typeof(string), typeof(int?), typeof(int?) };
                    constructorParamsValues = new object[] { Convert.ToString(args[0]), Convert.ToInt32(args[1]), Convert.ToString(args[2]), Convert.ToString(args[3]), (int?)args[4], (int?)args[5] };
                }
                else if (args.Length == 8)
                {
                    constructorParamsTypes = new Type[] { typeof(string), typeof(int), typeof(string), typeof(string), typeof(int?), typeof(int?), typeof(bool?), typeof(int?) };
                    constructorParamsValues = new object[] { Convert.ToString(args[0]), Convert.ToInt32(args[1]), Convert.ToString(args[2]), Convert.ToString(args[3]), (int?)args[4], (int?)args[5], (bool?)args[6], (int?)args[7] };
                }
                else if (args.Length == 9)
                {
                    constructorParamsTypes = new Type[] { typeof(string), typeof(int), typeof(string), typeof(string), typeof(int?), typeof(int?), typeof(bool?), typeof(int?), typeof(NegotiationSetting) };
                    constructorParamsValues = new object[] { Convert.ToString(args[0]), Convert.ToInt32(args[1]), Convert.ToString(args[2]), Convert.ToString(args[3]), (int?)args[4], (int?)args[5], (bool?)args[6], (int?)args[7], args[8] as NegotiationSetting };
                }
                else if (args.Length == 10)
                {
                    constructorParamsTypes = new Type[] { typeof(string), typeof(int), typeof(string), typeof(string), typeof(int?), typeof(int?), typeof(bool?), typeof(int?), typeof(NegotiationSetting), typeof(TimingSetting) };
                    constructorParamsValues = new object[] { Convert.ToString(args[0]), Convert.ToInt32(args[1]), Convert.ToString(args[2]), Convert.ToString(args[3]), (int?)args[4], (int?)args[5], (bool?)args[6], (int?)args[7], args[8] as NegotiationSetting, args[9] as TimingSetting };
                }
            }
            else
            {
                if (args.Length == 7)
                {
                    constructorParamsTypes = new Type[] { typeof(string), typeof(int), typeof(int), typeof(string), typeof(string), typeof(int?), typeof(int?) };
                    constructorParamsValues = new object[] { Convert.ToString(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), Convert.ToString(args[3]), Convert.ToString(args[4]), (int?)args[5], (int?)args[6] };
                }
                else if (args.Length == 8)
                {
                    constructorParamsTypes = new Type[] { typeof(string), typeof(int), typeof(int), typeof(string), typeof(string), typeof(int?), typeof(int?), typeof(NegotiationSetting) };
                    constructorParamsValues = new object[] { Convert.ToString(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), Convert.ToString(args[3]), Convert.ToString(args[4]), (int?)args[5], (int?)args[6], args[7] as NegotiationSetting};

                }
                else if (args.Length == 9)
                {
                    constructorParamsTypes = new Type[] { typeof(string), typeof(int), typeof(int), typeof(string), typeof(string), typeof(int?), typeof(int?), typeof(NegotiationSetting), typeof(TimingSetting) };
                    constructorParamsValues = new object[] { Convert.ToString(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), Convert.ToString(args[3]), Convert.ToString(args[4]), (int?)args[5], (int?)args[6], args[7] as NegotiationSetting, args[8] as TimingSetting };
                }
            }

            sessionConstructor = sessionType.GetConstructor(constructorParamsTypes);

            if (sessionConstructor == null)
                throw new MissingMethodException("El Protocolo Alpha no dispone del constructor esperado");

            return sessionConstructor.Invoke(constructorParamsValues) as ISession;
        }
    }
}

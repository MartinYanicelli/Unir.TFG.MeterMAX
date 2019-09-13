using Noanet.XamArch.Domain;
using Noanet.XamArch.Domain.PersistenceSupport;
using Noanet.XamArch.Logic;
using System;
using System.Threading.Tasks;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Logic.Contracts;
using Unir.TFG.MeterMAX.MDM.Logic.Validations;
using Unir.TFG.MeterMAX.MDM.Repository.Interfaces;

namespace Unir.TFG.MeterMAX.MDM.Logic
{
    public class AccountManager : IAccountManager
    {
        private readonly IUserSessionRepository userSessionRepository;
        private readonly IUserRepository userRepository;

        public UserSession CurrentSession { get; private set; }

        public AccountManager(IUserRepository userRepository, IUserSessionRepository userSessionRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.userSessionRepository = userSessionRepository ?? throw new ArgumentNullException(nameof(userSessionRepository));
        }

        public void LogOff()
        {
            if (CurrentSession == null)
                return;

            try
            {
                userSessionRepository.DbContext.OpenConnection();
                var userSesssion = CurrentSession;
                userSesssion.EndDate = DateTime.UtcNow;
                userSessionRepository.SaveOrUpdate(userSesssion);
            }
            catch (Exception)
            {
                // loguear excepción.
            }
            finally
            {
                CurrentSession = null;
                userSessionRepository.DbContext.CloseConnection();
            }
        }

        public async Task<AuthenticationResult> LogOnAsync(string userName, string password)
        {
            return await System.Threading.Tasks.Task.Run(() =>
            {
                //    CurrentSession = new UserSession()
                //    {
                //        SessionStarted = DateTime.UtcNow,
                //        Id = 1,
                //        User = new User()
                //        {
                //            Id = 1,
                //            Identication = "12345678",
                //            UserName = "Admin",
                //            Password = "1234",
                //            Role = new Role()
                //            {
                //                Id = 1,
                //                Name = "Administrator"
                //            }
                //        }
                //    };
                //    return new AuthenticationResult() {
                //         ValidationResult = null, UserSession = CurrentSession
                //    };

                AuthenticationResult result = new AuthenticationResult()
                {
                    //TODO: encriptar password!!
                    ValidationResult = IsValid(userName, password)
                };
                
                if (result.ValidationResult == ValidationResult.Success)
                {
                    IDbContext dbContext = userRepository.DbContext;
                    try
                    {
                        dbContext.OpenConnection();
                        User user = userRepository.FindOne(new EntityPropertyInfo(nameof(User.UserName), userName), new EntityPropertyInfo(nameof(User.Password), password));
                        if (!user?.IsTransient() ?? false)
                        {
                            // autenticación exitosa.
                            result.UserSession = new UserSession()
                            {
                                StartDate = DateTime.UtcNow,
                                User = user
                            };
                            userSessionRepository.SaveOrUpdate(result.UserSession);
                            CurrentSession = result.UserSession;
                        }
                        else
                        {
                            result.ValidationResult = new ValidationResult("Las credenciales de autenticación no son válidas.");
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        dbContext.CloseConnection();
                    }
                }
                return result;
            });
        }

        private ValidationResult IsValid(string userName, string password)
        {
            ValidationResult result = ValidationResult.Success;
            // regla de negocio que define la longitud máxima y mínima tanto el nombre de usuario como de su contraseña.
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                result = new ValidationResult("Las credenciales no pueden ser vacías o nulas.");
            }
            else if ((userName.Length < 5 || userName.Length > 12) || (password.Length < 4 || password.Length > 8))
            {
                result = new ValidationResult("Las Credenciales no tienen un formato válido.");
                if (userName.Length < 5 || userName.Length > 12)
                {
                    result.MemberNames.Add(nameof(userName), "El Nombre de Usuario debe estar comprendido entre 6 a 12 caracteres");
                }
                if (password.Length < 4 || password.Length > 8)
                {
                    result.MemberNames.Add(nameof(password), "La contraseña debe estar comprendida entre 4 a 8 caracteres.");
                }
            }

            return result;
            
        }
    }
}

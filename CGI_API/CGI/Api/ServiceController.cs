using CGI.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CGI.BAL;
using System.Transactions;
using CGI.BAL.Repository;
using System.Collections;
namespace CGIAPI.Api
{
    public class ServiceController : ApiController
    {

        #region [Registration/Login]

        [HttpPost]
        [Route("RegisterUser")]
        public void RegisterUser(UserRegister obj)
        {
            if (obj != null)
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    try
                    {
                        using (RepsistoryEF<UserRegister> _o = new global::RepsistoryEF<UserRegister>())
                        {
                            UserRegister ur = null;
                            obj.CreateDate = DateTime.Now;
                            if (obj.RegistrationID > 0)
                            {
                                ur = _o.Update(obj);
                            }
                            else
                            {
                                ur = _o.Save(obj);
                            }

                            ReturnValues result = null;
                            if (ur != null)
                            {
                                result = new ReturnValues
                                {
                                    Success = "Registeration Successfully Done ",
                                };
                            }
                            trans.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Dispose();
                        ReturnValues objex = new ReturnValues
                        {
                            Failure = ex.Message,

                        };
                        throw ex;
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }
        }

        [HttpPost]
        [Route("LoginUser")]
        public ReturnValues LoginUser(Login obj)
        {
            try
            {
                ReturnValues result = null;
                using (RepsistoryEF<UserRegister> _o = new RepsistoryEF<UserRegister>())
                {
                    var resultValue = _o.GetListBySelector(z => z.UserName == obj.UserName.Trim() && z.Password == obj.Password.Trim()).FirstOrDefault();

                    if (resultValue != null)
                    {
                        if (obj.GCMId != null && obj.GCMId != string.Empty)
                        {
                            resultValue.GCMId = obj.GCMId;
                            _o.Update(resultValue);
                        }
                        result = new ReturnValues
                        {
                            Success = "Login Successfully",
                            Source = resultValue.RegistrationID.ToString(),
                        };
                    }
                    else
                    {
                        result = new ReturnValues
                        {
                            Success = "Login Failed, Please enter correct username and password",
                            Source = "0",
                        };
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                ReturnValues objex = new ReturnValues
                {
                    Failure = ex.Message,

                };
                throw ex;
            }
            finally
            {

            }

        }

        [HttpPost]
        [Route("ChangePassword")]
        public ReturnValues ChangePassword(ChangeUserPassword obj)
        {
            try
            {
                ReturnValues result = null;
                if (obj != null)
                {
                    using (RepsistoryEF<UserRegister> _o = new RepsistoryEF<UserRegister>())
                    {
                        var resultValue = _o.GetListBySelector(z => z.RegistrationID == obj.RegistrationID).FirstOrDefault();

                        if (resultValue != null)
                        {
                            if (resultValue.Password == obj.OldPassword)
                            {
                                resultValue.Password = obj.NewPassword;
                                _o.Update(resultValue);
                                result = new ReturnValues
                                {
                                    Success = "Password Changed Successfully",
                                    Source = resultValue.RegistrationID.ToString(),
                                };
                            }
                            else
                            {
                                result = new ReturnValues
                                {
                                    Success = "Old password is not correct.<br/>Please try again.",
                                    Source = resultValue.RegistrationID.ToString(),
                                };
                            }
                        }
                        else
                        {
                            result = new ReturnValues
                            {
                                Success = "Password could not be changed.",
                                Source = "0",
                            };
                        }
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                ReturnValues objex = new ReturnValues
                {
                    Failure = ex.Message,

                };
                throw ex;
            }
            finally
            {

            }

        }

        [HttpGet]
        [Route("GetUserInfo")]
        public UserRegister GetUserInfo(int UserID)
        {
            UserRegister user = null;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    if (UserID != 0)
                    {
                        using (RepsistoryEF<UserRegister> _o = new RepsistoryEF<UserRegister>())
                        {
                            user = _o.GetListBySelector(x => x.RegistrationID.Equals(UserID)).FirstOrDefault();
                        }
                    }

                    trans.Complete();
                    return user;
                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    throw ex;
                }
                finally
                {
                    trans.Dispose();
                }
            }
        }

        [HttpGet]
        [Route("GetForgotPassword")]
        public string GetForgotPassword(string email)
        {
            string response = string.Empty;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    if (!string.IsNullOrEmpty(email))
                    {
                        using (RepsistoryEF<UserRegister> _o = new RepsistoryEF<UserRegister>())
                        {
                            var user = _o.GetListBySelector(x => x.Email.Equals(email)).FirstOrDefault();
                            if (user != null)
                            {
                                reposSendMail objreposSendMail = new reposSendMail();
                                objreposSendMail.contentBody(user, "ForgetPassword");
                                response = "Password has been sent successfully";
                            }
                            else
                            {
                                response = "Email address is not registered.";
                            }
                        }
                    }

                    trans.Complete();
                    return response;
                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    throw ex;
                }
                finally
                {
                    trans.Dispose();
                }
            }
        }


        #endregion
      
    }
}




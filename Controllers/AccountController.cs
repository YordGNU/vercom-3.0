using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using vercom.Models;

namespace Inspinia_MVC5.Controllers
{
    public class AccountController : Controller
    {
        private VERCOMEntities db = new VERCOMEntities();

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, string returnUrl)
        {
            if (Membership.ValidateUser(username, password))
            {

                // User is valid, create authentication ticket
                FormsAuthentication.SetAuthCookie(username, false);              
                if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // Invalid login
            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Users model, string PasswordConfirma)
        {
            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: PasswordConfirma, null, true, null, out createStatus);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Users model, string oldpassword, string newpassword)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(oldpassword, newpassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        // Helper to convert status to error message
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "El usuario ya existe. Por favor entre otro nombre de usuario.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Ya se ha registrado un usuario con el correo especificado. Por favor entre otra dirección de correo.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Contraseña invalida. Por favor entre una clave valida.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Correo invalido. Por favor entre un correo válido.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "La respuesta de recuperación de contraseña proporcionada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "La pregunta de recuperación de contraseña proporcionada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidUserName:
                    return "El nombre de usuario proporcionado no es válido. Compruebe el valor y vuelva a intentarlo.";

                case MembershipCreateStatus.ProviderError:
                    return "El proveedor de autenticación devolvió un error. Por favor, verifique su entrada y vuelva a intentarlo. Si el problema persiste, póngase en contacto con el administrador del sistema.";

                case MembershipCreateStatus.UserRejected:
                    return "La solicitud de creación de usuario se ha cancelado. Por favor, verifique su entrada y vuelva a intentarlo. Si el problema persiste, póngase en contacto con el administrador del sistema.";

                default:
                    return "Se ha producido un error desconocido. Por favor, verifique su entrada y vuelva a intentarlo. Si el problema persiste, póngase en contacto con el administrador del sistema.";
            }
        }

        public ActionResult Unauthorised()
        {
            return View();
        }
    }
}
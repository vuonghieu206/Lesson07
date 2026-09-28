using Demo7.Models.DataModels;
using Demo7.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Example05.Controllers
{
    public class MemberController : Controller
    {
        public static readonly List<Member> members = new List<Member>();

        public IActionResult Index()
        {
            return View(members);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                Member m = new Member
                {
                    MemberId = Guid.NewGuid().ToString(),
                    UserName = register.UserName,
                    FullName = register.FullName,
                    Password = register.Password,
                    Email = register.Email,
                    Phone = register.Phone,
                    Birthday = register.Birthday
                };

                members.Add(m);

                return RedirectToAction("Index");
            }

            return View(register);
        }
    }
}
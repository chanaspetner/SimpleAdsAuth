using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleAdsAuthWeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SimpleAdsAuthData;

namespace SimpleAdsAuthWeb.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=MyFirstDatabase;Integrated Security=true;";

        public IActionResult Index()
        {
            var repo = new UserRepository(_connectionString);
            var vm = new HomeIndexViewModel();
            vm.Ads = repo.GetAddsByDateDesc();
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;
                vm.CurrentUser = repo.GetByEmail(email);
            }
            return View(vm);
        }

        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var email = User.Identity.Name;
            var repo = new UserRepository(_connectionString);
            var user = repo.GetByEmail(email);
            ad.UserId = user.Id;
            ad.Name = user.Name;
            repo.NewAd(ad);
            return RedirectToAction("index");
        }

        [HttpPost]
        public IActionResult DeleteAd(int Id)
        {

            return RedirectToAction("index");
        }

        [Authorize]
        public IActionResult MyAccount()
        {
            var email = User.Identity.Name;
            var repo = new UserRepository(_connectionString);
            var user = repo.GetByEmail(email);
            return View(new MyAccountViewModel
            {
                Ads = repo.GetAddsByDateDescByUserId(user.Id)
            });
        }
    }
}

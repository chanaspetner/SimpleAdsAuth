using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleAdsAuthData;

namespace SimpleAdsAuthWeb.Models
{
    public class HomeIndexViewModel
    {
        public List<Ad> Ads { get; set; }
        public User CurrentUser { get; set; }
    }
}

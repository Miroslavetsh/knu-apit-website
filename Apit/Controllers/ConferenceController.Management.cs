﻿using System;
using System.Threading.Tasks;
using BusinessLayer.Models;
using DatabaseLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Apit.Controllers
{
    public partial class ConferenceController
    {
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> Create(NewConferenceViewModel model)
        {
            // TODO: new conference validation

            var dateNow = new DateTime();

            var conference = new Conference // I propose to use constructor instead of the class instance initialization to avoid the code duplication
            {
                Id = Guid.NewGuid(),
                UniqueAddress = model.UniqueAddress,
                IsActual = true,
                Title = model.Title,

                ShortDescription = model.ShortDescription,
                Description = model.Description,

                DateCreated = dateNow,
                DateLastModified = dateNow,
                DateStart = dateNow,
                DateFinish = dateNow
            };

            var user = await _userManager.GetUserAsync(User);
            _dataManager.Conferences.AddAdmin(user); // Why Admin? How you distinguish user with admin role and the rest users?
            _dataManager.Conferences.Create(conference);

            return View(model);
        }

        [Authorize]
        public IActionResult Update()
        {
            return View();
        }

        [Authorize, HttpPost]
        public IActionResult Delete()
        {
            return RedirectToAction("index", "conference");
        }
    }
}
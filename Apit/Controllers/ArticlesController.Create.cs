﻿using System;
using System.Threading.Tasks;
using Apit.Service;
using BusinessLayer.DataServices;
using BusinessLayer.Models;
using DatabaseLayer.Entities;
using DatabaseLayer.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Apit.Controllers
{
    public partial class ArticlesController
    {
        [Authorize] //Apply the [Authorize] attribute to the whole class instead of the each method.
        public IActionResult Create()
        {
            return View(new NewArticleViewModel
            {
                UniqueAddress = _dataManager.Articles.GenerateUniqueAddress()
            });
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Create(NewArticleViewModel model)
        {
            // Combine all errors and return them back if this variable is set to true
            bool hasIncorrectData = false;

            // These checks better to perform using the appropriate ViewModel class and the appropriate attributes (e.g. Required, Remote)
            #region ================ Long form review code ================

            // Check unique address field value
            model.UniqueAddress.NormalizeAddress();

            if (model.UniqueAddress.Length < 5)
            {
                ModelState.AddModelError(nameof(NewArticleViewModel.UniqueAddress),
                    "адреса закоротка (потрібна довжина 5-25 символів)");
                hasIncorrectData = true;
            }
            else if (model.UniqueAddress.Length > 25)
            {
                ModelState.AddModelError(nameof(NewArticleViewModel.UniqueAddress),
                    "адреса задовга (потрібна довжина 5-20 символів)"); // Either the length should be up to 20 symbols or up to 25. Not 20 and 25 symbols simultaneously.
                hasIncorrectData = true;
            }
            else if (_dataManager.Articles.GetByUniqueAddress(model.UniqueAddress) != null)
            {
                ModelState.AddModelError(nameof(NewArticleViewModel.UniqueAddress),
                    "ця адреса вже використовується, оберіть іншу");
                hasIncorrectData = true;
            }


            // Check selected topic existing
            var topic = _dataManager.Topics.GetById(Guid.Parse(model.TopicId));
            if (topic == null)
            {
                ModelState.AddModelError(nameof(model.TopicId),
                    "дана опція не може бути використана");
                hasIncorrectData = true;
            }

            // Check uploaded file
            var extension = DataUtil.GetExtension(model.DocFile.FileName);
            if (model.DocFile.Length > 0)
            {
                if (extension != "doc" && extension != "docx")
                {
                    ModelState.AddModelError(nameof(model.DocFile),
                        "невірний формат файлу (доступно лише .doc і .docx)");
                    hasIncorrectData = true;
                }
                else if (!hasIncorrectData)
                {
                    if (!await DataUtil.TrySaveDocFile(model.DocFile, model.UniqueAddress, extension))
                    {
                        ModelState.AddModelError(nameof(model.DocFile),
                            "даний файл не може бути збереженим, оскільки може нести у собі загрозу для сервісу. " +
                            "Якщо це не так, будь ласка, зверніться до адміністрації сайту");
                        hasIncorrectData = true;
                    }
                }
            }
            else
            {
                ModelState.AddModelError(nameof(model.DocFile),
                    "будь ласка, прикріпіть файл з матеріалом"); // typo
                hasIncorrectData = true;
            }

            #endregion

            if (hasIncorrectData) return View(model);


            var dateNow = DateTime.Now;
            var user = await _userManager.GetUserAsync(User);

            var article = new Article
            {
                Id = Guid.NewGuid(),
                TopicId = topic.Id,
                CreatorId = user.Id,

                Title = model.Title,
                Status = ArticleStatus.Uploaded,
                KeyWords = _keyWordsSeparatorRegex.Replace(model.KeyWords, ";"),

                HtmlFilePath = model.UniqueAddress + ".htm",
                DocxFilePath = model.UniqueAddress + "." + extension,

                Conference = _dataManager.Conferences.GetCurrentAsDbModel(),

                DateCreated = dateNow,
                DateLastModified = dateNow
            };
            
            _dataManager.Conferences.AddArticle(article);
            _dataManager.Articles.Create(article);

            return Redirect("/articles/p/" + model.UniqueAddress);
        }
    }
}
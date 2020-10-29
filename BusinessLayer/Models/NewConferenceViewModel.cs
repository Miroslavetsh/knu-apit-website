﻿using System;
using System.ComponentModel.DataAnnotations;
using DatabaseLayer;
using DatabaseLayer.Entities;

namespace BusinessLayer.Models
{
    public class NewConferenceViewModel
    {
        [Required(ErrorMessage = MSG.OnRequired)]
        [Display(Name = MSG.UniqueAddressName)]
        public string UniqueAddress { get; set; }


        [Required(ErrorMessage = MSG.OnRequired)]
        [Display(Name = "Короткий опис")]
        public string Title { get; set; }


        [Required(ErrorMessage = MSG.OnRequired)]
        [Display(Name = "Короткий опис")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = MSG.OnRequired)]
        [Display(Name = "Повний опис")]
        public string Description { get; set; }


        [Required(ErrorMessage = "Вкажіть як мінімум одну тему")]
        public string[] Topics { get; set; }

        [Display(Name = "Адміністрація конференції")]
        public ConferenceAdmin[] Admins { get; set; }

        
        
        [Required(ErrorMessage = MSG.OnRequired)]
        [DataType(DataType.DateTime)]
        public DateTime DateStart { get; set; }

        [Required(ErrorMessage = MSG.OnRequired)]
        [DataType(DataType.DateTime)]
        public DateTime DateFinish { get; set; }
    }
}
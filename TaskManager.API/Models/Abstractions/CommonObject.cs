﻿using System;
using TaskManager.Common.Models;

namespace TaskManager.API.Models
{
    public abstract class CommonObject
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }
        public byte[]? Image { get; set; }
        public CommonObject() => CreationDate = DateTime.Now;

        public CommonObject(CommonModel model)
        {
            Name = model.Name;
            Description = model.Description;
            Image = model.Image;
            CreationDate = model.CreationDate;
            Image = model.Image;
        }
    }
}

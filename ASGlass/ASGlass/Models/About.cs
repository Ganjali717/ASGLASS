﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ASGlass.Models
{
    public class About
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 1000)]

        public string Title { get; set; }
        [StringLength(maximumLength: 100)]

        public string Image { get; set; }

        [NotMapped]
        public IFormFile PosterFile { get; set; }
    }
}

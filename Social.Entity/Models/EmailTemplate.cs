using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Social.Entity.Models
{
    public class EmailTemplate
    {
        public Guid Id { get; set; }
        public string HtmlTemplate { get; set; }
        public string TemplateCode { get; set; }
    }
}

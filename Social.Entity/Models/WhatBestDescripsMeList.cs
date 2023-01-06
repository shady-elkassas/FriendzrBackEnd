using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
    public class WhatBestDescripsMeList
    {
        public int Id { get; set; }
        public string EntityId { get; set; }
        public string Tagsname { get; set; }
        public int? UserId { get; set; }
        public int WhatBestDescripsMeId { get; set; }
        public virtual UserDetails User { get; set; }
        public virtual WhatBestDescripsMe WhatBestDescripsMe { get; set; }
    }
}

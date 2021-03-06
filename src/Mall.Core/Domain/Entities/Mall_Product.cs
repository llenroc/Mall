﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Mall.Domain.Entities
{
    /// <summary>
    /// 产品名称
    /// </summary>
    public class Mall_Product : FullAuditedEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(40)]
        public string ItemNo { get; set; }

        [StringLength(1000)]
        public string Describe { get; set; }

        [Required]
        public decimal Price { get; set; }
        /// <summary>
        /// 上传的主要图片
        /// </summary>
        [Required]
        public string ImgPic { get; set; }

        /// <summary>
        /// 外键
        /// </summary>
        [ForeignKey("Mall_Categories")]
        [Required]
        public int CategoryId { get; set; }

        public virtual Mall_Category Category { get; set; }

        public Mall_Product()
        {
            Price = 0;
            IsDeleted = false;
        }
    }

    
}

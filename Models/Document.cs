using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentStorageSystem.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public byte[] Content { get; set; }

        public int Version { get; set; }

        public DateTime UploadDate { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}

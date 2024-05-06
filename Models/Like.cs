using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCoreTheHive.Models
{
    [Table("Likes")]
    public class Like
    {
        [Key]
        [Column("id_publicacion")]
        public int IdPublicacion { get; set; }

        [Key]
        [Column("username")]
        public string Username { get; set; }

        [ForeignKey("IdPublicacion")]
        public Publicacion Publicacion { get; set; }

        [ForeignKey("Username")]
        public Usuario Usuario { get; set; }
    }
}

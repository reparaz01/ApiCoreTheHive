using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCoreTheHive.Models
{
    [Table("Publicaciones")]
    public class Publicacion
    {
        [Key]
        [Column("id")]
        public int IdPublicacion { get; set; }

        [Column("texto")]
        public string Texto { get; set; }

        [Column("imagen")]
        public string Imagen { get; set; }

        [Column("foto_perfil")]
        public string FotoPerfil { get; set; }

        [Column("fecha_publicacion")]
        public DateTime FechaPublicacion { get; set; }

        [Column("tipo_publicacion")]
        public int TipoPublicacion { get; set; }

        [Column("Likeado")]
        public bool Likeado { get; set; }

        [ForeignKey("Usuario")]
        public string Username { get; set; }



    }
}

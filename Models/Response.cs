using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    [Table("responses")]
    public class Response
    {

        [Key]
        public int ResponseId {get;set;}


        public int UserId {get;set;}


        public int WeddingId {get;set;}


        public User Guest {get;set;}
        
    }
}
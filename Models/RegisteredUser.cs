﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PROG6212POE.Models
{
    [Table("Registered_User")]
    public partial class RegisteredUser
    {
        public RegisteredUser()
        {
            Modules = new HashSet<Module>();
            Semesters = new HashSet<Semester>();
        }

        [Key]
        [Column("users_id")]
        public int UsersId { get; set; }
        [Required]
        [Column("username")]
        [StringLength(250)]
        [Unicode(false)]
        public string Username { get; set; }
        [Required]
        [Column("password")]
        [StringLength(250)]
        [Unicode(false)]
        public string Password { get; set; }

        [InverseProperty("Users")]
        public virtual ICollection<Module> Modules { get; set; }
        [InverseProperty("Users")]
        public virtual ICollection<Semester> Semesters { get; set; }
    }
}
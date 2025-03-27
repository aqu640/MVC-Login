using System;
using System.ComponentModel.DataAnnotations;

namespace UsersApp.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The First Name is required")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "The Last Name is required")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "The Email is required")]
        public string Email { get; set; } = "";

        public string? Phone { get; set; } = "";

        public string? Address { get; set; } = "";

        [Required(ErrorMessage = "The Company is required")]
        public string Company { get; set; } = "";

        public string? Notes { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
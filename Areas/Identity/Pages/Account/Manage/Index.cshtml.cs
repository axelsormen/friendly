// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using friendly.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace friendly.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public string ProfileImageUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual List<Post>? Posts { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [RegularExpression(@"[a-zA-ZæøåÆØÅ -]{2,30}", ErrorMessage = "The name must be letters and between 2 to 30 characters.")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [RegularExpression(@"[a-zA-ZæøåÆØÅ -]{2,30}", ErrorMessage = "The name must be letters and between 2 to 30 characters.")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            Username = await _userManager.GetUserNameAsync(user);
            Email = await _userManager.GetEmailAsync(user);

            // Access custom properties directly
            FirstName = user.FirstName;
            LastName = user.LastName;
            ProfileImageUrl = user.ProfileImageUrl;
            Posts = user.Posts;

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Input = new InputModel
            {
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

    public async Task<IActionResult> OnPostAsync(IFormFile ProfileImageFile)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        // Handle profile image upload
        if (ProfileImageFile != null && ProfileImageFile.Length > 0)
        {
            // Check the file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(ProfileImageFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                // Add a model state error for invalid file extension
                ModelState.AddModelError("ProfileImageFile", "Only .jpg, .jpeg, and .png files are allowed.");
                StatusMessage = "Only .jpg, .jpeg, and .png files are allowed.";
                await LoadAsync(user); // Load user data before returning the page
                return Page();
            }

            // Proceed with file saving
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/profile-images");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ProfileImageFile.FileName);
            var filePath = Path.Combine(uploadsDir, uniqueFileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImageFile.CopyToAsync(stream);
                }

                // Assign the file path to ProfileImageUrl
                user.ProfileImageUrl = $"/uploads/profile-images/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ProfileImageFile", $"An error occurred while saving the image: {ex.Message}");
                return Page();
            }
        }

        // Update other properties and save changes
        if (Input.FirstName != user.FirstName)
        {
            user.FirstName = Input.FirstName;
        }

        if (Input.LastName != user.LastName)
        {
            user.LastName = Input.LastName;
        }

        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (Input.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set phone number.";
                return RedirectToPage();
            }
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            StatusMessage = "Unexpected error when trying to update user.";
            return RedirectToPage();
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }
    }
}
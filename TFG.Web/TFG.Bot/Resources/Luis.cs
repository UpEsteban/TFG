﻿using System.Collections.Generic;
using System.Globalization;

namespace TFG.Bot.Resources
{
    public class Luis
    {
        public const string Welcome_Intent = "Welcome";
        public const string ProfileAddAllergy_Intent = "Profile_AddAllergy";
        public const string ProfileDeleteAllergy_Intent = "Profile_DeleteAllergy";
        public const string ProfileDeleteUser_Intent = "Profile_DeleteUser";
        public const string ProfileMyAllergies_Intent = "Profile_MyAllergies";
        public const string RecipeAllergy_Intent = "Recipe_Allergy";
        public const string RecipeSearch_Intent = "Recipe_Search";
        public const string None_Intent = "None";

        public const string SubDialogs = "SubDialogs";
        public const string AllergyEntity = "Allergy";


        public static readonly List<string> ValidAllergies = new List<string> { "Cacahuete", "Crustáceos", "Frutos secos", "Gluten", "Huevo", "Leche", "Pescado", "Soja" };
    }
}

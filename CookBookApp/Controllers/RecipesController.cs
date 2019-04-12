﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookBookApp.Data;
using CookBookApp.Models;
using CookBookApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CookBookApp.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public RecipesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public IActionResult Create()
        {
            var vm = new CreateRecipeViewModel()
            {
                MealCategories = context.Categories.OrderBy(c => c.Name).ToList(),
                Ingredients = context.Ingredients.OrderBy(i => i.Name).ToList(),
                Recipe = new Recipe(),
                ChosenIngredients = new List<IngredientInRecipe>()
            };

            return View(vm);
        }


        public IActionResult CreateRecipe(CreateRecipeViewModel vm)
        {
            var recipe = vm.Recipe;
            recipe.CreatedAt = DateTime.Now;
            recipe.UserId = userManager.GetUserId(User);

            context.Recipes.Add(recipe);
            context.SaveChanges();

            var id = recipe.Id;

            foreach (var ingredient in vm.ChosenIngredients)
            {
                var ing = new IngredientInRecipe()
                {
                    IngredientId = ingredient.IngredientId,
                    Quantity = ingredient.Quantity,
                    RecipeId = id
                };

                context.IngredientsInRecipes.Add(ing);
            }

            context.SaveChanges();

            return Json(new { redirectToUrl = Url.Action("Index", "Home") });
        }
    }
}
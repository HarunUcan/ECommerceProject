using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.CartDtos;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers;

public abstract class BaseUserController : Controller
{
    protected BaseUserController(ICategoryService categoryService, ICartService cartService, UserManager<AppUser> userManager)
    {
        CategoryService = categoryService;
        CartService = cartService;
        UserManager = userManager;
    }

    protected ICategoryService CategoryService { get; }

    protected ICartService CartService { get; }

    protected UserManager<AppUser> UserManager { get; }

    protected async Task<UserViewContext> BuildUserViewContextAsync()
    {
        var categories = CategoryService.TGetList();
        var user = await UserManager.GetUserAsync(User);
        var cart = await CartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user?.Id ?? 0);
        return new UserViewContext(categories, cart, user);
    }

    protected record UserViewContext(List<Category> Categories, CartDetailDto? Cart, AppUser? User);
}

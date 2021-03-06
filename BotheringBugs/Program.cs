using BotheringBugs.Data;
using BotheringBugs.Models;
using BotheringBugs.Services;
using BotheringBugs.Services.Factories;
using BotheringBugs.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(DataUtility.GetConnectionString(builder.Configuration),
     o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))); 

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<BBUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<BBUserClaimsPrincipalFactory>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

// Custom Services
builder.Services.AddScoped<IBBRolesService, BBRoleService>();
builder.Services.AddScoped<IBBCompanyInfoService, BBCompanyInfoService>();
builder.Services.AddScoped<IBBProjectService, BBProjectService>();
builder.Services.AddScoped<IBBTicketService, BBTickerService>();
builder.Services.AddScoped<IBBTicketHistoryService, BBTicketHistoryService>();
builder.Services.AddScoped<IBBNotificationService, BBNotificationService>();
builder.Services.AddScoped<IBBInviteService, BBInviteService>();
builder.Services.AddScoped<IBBFileService, BBFileService>();
builder.Services.AddScoped<IBBLookUpService, BBLookUpService>(); 

builder.Services.AddScoped<IEmailSender, BBEmailService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddControllersWithViews();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

await DataUtility.ManageDataAsync(app);

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.Run();

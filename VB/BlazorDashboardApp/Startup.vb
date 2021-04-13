Imports DevExpress.DashboardAspNetCore
Imports DevExpress.DashboardCommon
Imports DevExpress.DashboardWeb
Imports DevExpress.DataAccess.Json
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.FileProviders
Imports Microsoft.Extensions.Hosting
Imports System

Namespace BlazorDashboardApp
	Public Class Startup
		Public Sub New(ByVal configuration As IConfiguration, ByVal hostingEnvironment As IWebHostEnvironment)
			Me.Configuration = configuration
			FileProvider = hostingEnvironment.ContentRootFileProvider
		End Sub

		Public ReadOnly Property Configuration() As IConfiguration
		Public ReadOnly Property FileProvider() As IFileProvider

		' This method gets called by the runtime. Use this method to add services to the container.
		' For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		Public Sub ConfigureServices(ByVal services As IServiceCollection)
			services.AddRazorPages()
			services.AddServerSideBlazor()
			services.AddMvc().AddDefaultDashboardController(Sub(configurator)
				configurator.SetDashboardStorage(New DashboardFileStorage(FileProvider.GetFileInfo("App_Data/Dashboards").PhysicalPath))
				configurator.SetDataSourceStorage(New DataSourceInMemoryStorage())
				configurator.SetConnectionStringsProvider(New DashboardConnectionStringsProvider(Configuration))
				AddHandler configurator.ConfigureDataConnection, AddressOf Configurator_ConfigureDataConnection
			End Sub)
		End Sub

		Private Sub Configurator_ConfigureDataConnection(ByVal sender As Object, ByVal e As ConfigureDataConnectionWebEventArgs)
			If e.DataSourceName.Contains("Support") Then
				Dim fileUri As New Uri(FileProvider.GetFileInfo("App_data/Support.json").PhysicalPath, UriKind.RelativeOrAbsolute)
				Dim jsonParams As New JsonSourceConnectionParameters()
				jsonParams.JsonSource = New UriJsonSource(fileUri)
				e.ConnectionParameters = jsonParams
			End If
			If e.DataSourceName.Contains("Categories") Then
				Dim fileUri As New Uri(FileProvider.GetFileInfo("App_data/Categories.json").PhysicalPath, UriKind.RelativeOrAbsolute)
				Dim jsonParams As New JsonSourceConnectionParameters()
				jsonParams.JsonSource = New UriJsonSource(fileUri)
				e.ConnectionParameters = jsonParams
			End If
			If e.DataSourceName.Contains("Customers") Then
				Dim jsonParams As New JsonSourceConnectionParameters()
				jsonParams.JsonSource = New UriJsonSource(New Uri("https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json"))
				e.ConnectionParameters = jsonParams
			End If
		End Sub

		' This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		Public Sub Configure(ByVal app As IApplicationBuilder, ByVal env As IWebHostEnvironment)
			If env.IsDevelopment() Then
				app.UseDeveloperExceptionPage()
			Else
				app.UseExceptionHandler("/Error")
				' The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts()
			End If

			app.UseHttpsRedirection()
			app.UseStaticFiles()

			app.UseRouting()

			app.UseEndpoints(Sub(endpoints)
				EndpointRouteBuilderExtension.MapDashboardRoute(endpoints, "api/dashboard")
				endpoints.MapBlazorHub()
				endpoints.MapFallbackToPage("/_Host")
			End Sub)
		End Sub
	End Class
End Namespace

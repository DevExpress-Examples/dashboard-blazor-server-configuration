using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.Json;
using Microsoft.Extensions.FileProviders;

namespace BlazorDashboardApp {
    public static class DashboardUtils {
        public static DashboardConfigurator CreateDashboardConfigurator(IConfiguration configuration, IFileProvider fileProvider) {
            DashboardConfigurator configurator = new DashboardConfigurator();
            configurator.SetDashboardStorage(new DashboardFileStorage(fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath));
            configurator.SetDataSourceStorage(new DataSourceInMemoryStorage());
            configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));

            configurator.ConfigureDataConnection += (s, e) => {                
                if (e.ConnectionName == "jsonSupport") {
                    Uri fileUri = new Uri(fileProvider.GetFileInfo("Data/Support.json").PhysicalPath, UriKind.RelativeOrAbsolute);
                    JsonSourceConnectionParameters jsonParams = new JsonSourceConnectionParameters();
                    jsonParams.JsonSource = new UriJsonSource(fileUri);
                    e.ConnectionParameters = jsonParams;
                }
                if (e.ConnectionName == "jsonCategories") {
                    Uri fileUri = new Uri(fileProvider.GetFileInfo("Data/Categories.json").PhysicalPath, UriKind.RelativeOrAbsolute);
                    JsonSourceConnectionParameters jsonParams = new JsonSourceConnectionParameters();
                    jsonParams.JsonSource = new UriJsonSource(fileUri);
                    e.ConnectionParameters = jsonParams;
                }
                if (e.ConnectionName == "jsonCustomers") {
                    JsonSourceConnectionParameters jsonParams = new JsonSourceConnectionParameters();
                    jsonParams.JsonSource = new UriJsonSource(
                        new Uri("https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json"));
                    e.ConnectionParameters = jsonParams;                    
                }
            };
            return configurator;
        }
    }
}
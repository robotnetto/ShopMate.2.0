using Microsoft.Extensions.Logging;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Domain.Exceptions;
using ShopMate._2._0.Domain.Interfaces;
using ShopMate._2._0.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShopMate._2._0.Applications.Services
{
    public class FoodDataService
    {
        private readonly IFoodDataRepository foodDataRepository;

        private List<FoodData> foodDataList;
        public FoodDataService(IFoodDataRepository foodDataRepository)
        {
            this.foodDataRepository = foodDataRepository;

        }

        public async Task PreloadDataAsync()
        {
            var result = await foodDataRepository.GetAllasync();

            if (!result.Any())
            {
                var jsonPath = Path.Combine(FileSystem.AppDataDirectory, "LivsmedelsDB_JSON.json");
                if (!File.Exists(jsonPath))
                {
                    // If the file does not exist, copy it from the MauiAsset folder
                    var assembly = Assembly.GetExecutingAssembly();
                    var resourceName = "ShopMate._2._0.Resources.Raw.LivsmedelsDB_JSON.json";

                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream == null)
                            throw new FileNotFoundException("JSON resource not found in assembly.");

                        using (var reader = new StreamReader(stream))
                        {
                            //logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Reading JSON content from embedded resource.");
                            var content = reader.ReadToEnd();
                            //logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> JSON content read successfully.");

                            // Save the content to the app's local storage
                            File.WriteAllText(jsonPath, content);
                            //logger.LogInformation($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> JSON content saved to {jsonPath}.");
                        }
                    }
                }

                var json = await File.ReadAllTextAsync(jsonPath);
                var jsonObject = JsonSerializer.Deserialize<Dictionary<string, List<FoodData>>>(json);

                if (jsonObject != null && jsonObject.ContainsKey("FoodData"))
                {
                    foodDataList = jsonObject["FoodData"];

                    foreach (var foodData in foodDataList)
                    {
                        await foodDataRepository.CreateAsync(foodData);
                    }
                }
            }
         
        }
        public async Task<List<FoodData>> GetPageDataAsync(int currentPage, int pageSize)
        {
            var result = await foodDataRepository.GetPageDataAsync(currentPage, pageSize);
            if (result == null)
            {
                throw new NotFoundException("No food data found!");
            }
            return result;
        }

        public async Task<IEnumerable<FoodData>> GetAllAsync()
        {
            var result = await foodDataRepository.GetAllasync();
            if (result == null)
            {
                throw new NotFoundException("No food data found!");
            }
            return result;
        }

    }
}




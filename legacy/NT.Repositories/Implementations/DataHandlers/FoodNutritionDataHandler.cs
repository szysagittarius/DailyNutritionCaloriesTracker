using AutoMapper;
using NT.Application.Contracts.Entities;
using NT.Application.Contracts.Ports;
using NT.Database.Entities;
using NT.Ef.Repositories.Abstractions;

namespace NT.Ef.Repositories.Implementations.DataHandlers;
public class FoodNutritionDataHandler : IFoodNutritionDataHandler
{
    private readonly IFoodNutritionRepository _foodNutritionRepository;
    private readonly IMapper _mapper;

    public FoodNutritionDataHandler(IFoodNutritionRepository foodNutritionRepository, IMapper mapper)
    {
        _foodNutritionRepository = foodNutritionRepository;
        _mapper = mapper;
    }
    public async Task<FoodNutritionEntity> AddAsync(FoodNutritionEntity foodNutrition)
    {
        FoodNutrition model = _mapper.Map<FoodNutrition>(foodNutrition);

        //check foodNutrition by name, if it already exists, skip adding it
        try
        {
            //FoodNutrition? existingModel = _foodNutritionRepository.GetAll()
            //.FirstOrDefault(fn => fn.Name.ToLower().Equals(foodNutrition.Name.ToLower()));

            //if (existingModel != null)
            //{
            //    return _mapper.Map<FoodNutritionEntity>(existingModel);
            //}

            FoodNutrition addedModel = await _foodNutritionRepository.AddAsync(model);
            return _mapper.Map<FoodNutritionEntity>(addedModel);

        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
        

       
    }

    public async Task DeleteAsync(string foodName)
    {
        FoodNutrition? foodNutrition = _foodNutritionRepository.GetAll()
            .FirstOrDefault(fn => fn.Name.Equals(foodName, StringComparison.OrdinalIgnoreCase));

        if (foodNutrition != null)
        {
            await _foodNutritionRepository.DeleteAsync(foodNutrition.Id);
        }
        else
        {
            throw new KeyNotFoundException($"No food nutrition found with name: {foodName}");
        }
    }

    public async Task DeleteAsync(IEnumerable<string> foodNames)
    {
        List<FoodNutrition> foodNutritions = _foodNutritionRepository.GetAll()
            .Where(fn => foodNames.Contains(fn.Name)).ToList();

        foreach (FoodNutrition? foodNutrition in foodNutritions)
        {
            await _foodNutritionRepository.DeleteAsync(foodNutrition.Id);
        }
    }

    public async Task DeleteAsync()
    {
        List<FoodNutrition> allFoodNutritions = _foodNutritionRepository.GetAll().ToList();
        foreach (FoodNutrition? foodNutrition in allFoodNutritions)
        {
            await _foodNutritionRepository.DeleteAsync(foodNutrition.Id);
        }
    }

    public async Task<FoodNutritionEntity> GetAsync(string foodName)
    {
        FoodNutrition? foodNutrition = _foodNutritionRepository.GetAll()
            .FirstOrDefault(fn => fn.Name.Equals(foodName, StringComparison.OrdinalIgnoreCase));

        return foodNutrition == null
            ? throw new KeyNotFoundException($"No food nutrition found with name: {foodName}")
            : _mapper.Map<FoodNutritionEntity>(foodNutrition);
    }

    public async Task<IEnumerable<FoodNutritionEntity>> GetAsync(IEnumerable<string> foodNames)
    {
        List<FoodNutrition> foodNutritions = _foodNutritionRepository.GetAll()
            .Where(fn => foodNames.Contains(fn.Name)).ToList();
        return _mapper.Map<IEnumerable<FoodNutritionEntity>>(foodNutritions);
    }

    public async Task<IEnumerable<FoodNutritionEntity>> GetAllAsync()
    {
        List<FoodNutrition> allFoodNutritions = _foodNutritionRepository.GetAll().ToList();
        return _mapper.Map<IEnumerable<FoodNutritionEntity>>(allFoodNutritions);
    }

    public async Task<FoodNutritionEntity> UpdateAsync(FoodNutritionEntity foodNutrition)
    {
        throw new NotImplementedException();
        //FoodNutrition existingModel = await _foodNutritionRepository.GetByIdAsync(foodNutrition.Id);
        //if (existingModel == null)
        //{
        //    throw new KeyNotFoundException($"No food nutrition found with ID: {foodNutrition.Id}");
        //}

        //_mapper.Map(foodNutrition, existingModel);
        //FoodNutrition updatedModel = await _foodNutritionRepository.UpdateAsync(existingModel);
        //return _mapper.Map<FoodNutritionEntity>(updatedModel);
    }
}
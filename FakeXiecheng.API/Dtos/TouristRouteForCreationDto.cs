using FakeXiecheng.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Dtos
{
    //[TouristRouteTitleMustBeDifferentFromDescriptionAttribute]
    public class TouristRouteForCreationDto: TouristRouteForManipulationDto // : IValidatableObject
    {
        //[Required(ErrorMessage = "Title cannot be null!")]
        //[MaxLength(100)]
        //public string Title { get; set; }
        //[Required(ErrorMessage = "Description cannot be null!")]
        //[MaxLength(1500)]
        //public string Description { get; set; }
        //// 计算方式: 原价 * 折扣
        //public decimal Price { get; set; }
        //public DateTime CreateTime { get; set; }
        //public DateTime? UpdateTime { get; set; }
        //public DateTime? DepartureTime { get; set; }
        //public string Features { get; set; }
        //public string Fees { get; set; }
        //public string Notes { get; set; }
        //public double? Rating { get; set; }

        //// 以下的三个变量,我们希望传递到前端的是字符串类型
        //public string TravelDays { get; set; }
        //public string TripType { get; set; }
        //public string DepartureCity { get; set; }
        //public ICollection<TouristRoutePictureForCreationDto> TouristRoutePictures { get; set; }
        //    = new List<TouristRoutePictureForCreationDto>();

        // Override the method of Validate from IValidatableObject
        //public IEnumerable<ValidationResult> Validate(
        //    ValidationContext validationContext)
        //{
        //    if(Title == Description)
        //    {
        //        yield return new ValidationResult(
        //            "Route title has to be different from its description.",
        //            new [] { "TouristRouteForCreationDto" }
        //        );
        //    }
        //}
    }
}

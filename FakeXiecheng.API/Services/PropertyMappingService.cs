﻿using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _touristRoutePropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "Id", new PropertyMappingValue(new List<string>(){ "Id" }) },
               { "Title", new PropertyMappingValue(new List<string>(){ "Title" })},
               { "Rating", new PropertyMappingValue(new List<string>(){ "Rating" })},
               { "OriginalPrice", new PropertyMappingValue(new List<string>(){ "OriginalPrice" })},
           };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(
                new PropertyMapping<TouristRouteDto, TouristRoute>(
                    _touristRoutePropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue>
            GetPropertyMapping<TSource, TDestination>()
        {
            // 获得匹配的映射对象
            var matchingMapping =
                _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().mappingDisctionary;
            }

            throw new Exception(
                $"Cannot find exact property mapping instance for <{typeof(TSource)},{typeof(TDestination)}");
        }

        // 通过检查Mapping中是否存在该关键词, 判断请求是否合法
        public bool IsMappingExists<TSource, TDestination>(String fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // 逗号来分隔字段字符串
            var fieldsAfterSplit = fields.Split(",");

            foreach(var field in fieldsAfterSplit)
            {
                // 去掉空格
                var trimmedField = field.Trim();
                // 获得属性名称字符串
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsPropertiesExists<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // 逗号来分隔字符串
            var fieldsAfterSplit = fields.Split(',');

            foreach(var field in fieldsAfterSplit)
            {
                // 获得属性名称字符串
                var propertyName = field.Trim();

                var propertyInfo = typeof(T)
                    .GetProperty(
                        propertyName,
                        BindingFlags.IgnoreCase | BindingFlags.Public
                        | BindingFlags.Instance
                    );
                // 如果T中没有找到对应的属性
                if(propertyInfo == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

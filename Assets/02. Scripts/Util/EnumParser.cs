using System;

public static class EnumParser
{
    /// <summary>
    /// 문자열을 enum 타입으로 변환 (대소문자 무시, 실패시 기본값 반환)
    /// </summary>
    /// <typeparam name="TEnum">enum 타입</typeparam>
    /// <param name="str">변환할 문자열</param>
    /// <param name="defaultValue">변환 실패 시 반환할 기본값 (없으면 default)</param>
    /// <returns>변환된 enum 값</returns>
    public static TEnum ParseOrDefault<TEnum>(string str, TEnum defaultValue = default) where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(str, ignoreCase: true, out var result))
        {
            return result;
        }
        return defaultValue;
    }

    /// <summary>
    /// enum 값을 문자열로 변환
    /// </summary>
    public static string ToString<TEnum>(TEnum enumValue) where TEnum : struct, Enum
    {
        return enumValue.ToString();
    }
}

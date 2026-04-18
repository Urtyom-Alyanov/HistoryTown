namespace HistoryTown.Core.Entities;

/// <summary>
/// Здание в городе (структура)
/// </summary>
/// <param name="Name">Наименование</param>
public record Structure(string Name);

/// <summary>
/// Улица в городе, соединяющая 2 здания
/// </summary>
/// <param name="From">От какого здания</param>
/// <param name="To">До какого здания</param>
/// <param name="Weight">Длина пути</param>
public record Street(Structure From, Structure To, double Weight = 1.0);

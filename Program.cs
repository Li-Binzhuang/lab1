using lab1;
using Microsoft.AspNetCore.Http.HttpResults;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 存储聚会信息的列表
var parties = new List<Party>();
// 已删除的聚会Id列表，用于避免Id重复使用
var deletedIds = new List<int>();

// 获取所有聚会信息
app.MapGet("/Party/All", () => Results.Ok(parties));

// 获取指定Id的聚会信息
app.MapGet("/Party/{id}", (int id) =>
{
    var party = parties.FirstOrDefault(p => p.Id == id);
    return party is null ? Results.NotFound() : Results.Ok(party);
});

// 添加新聚会
app.MapPost("/Party", (Party party) =>
{
    // 分配新的Id，避免使用已删除的Id
    int newId = parties.Count == 0 ? 1 : parties.Max(p => p.Id) + 1;
    while (deletedIds.Contains(newId))
    {
        newId++;
    }
    party.Id = newId;
    parties.Add(party);
    return Results.Created($"/Party/{party.Id}", party);
});

// 修改聚会信息
app.MapPut("/Party/{id}", (int id, Party updatedParty) =>
{
    var party = parties.FirstOrDefault(p => p.Id == id);
    if (party is null)
    {
        return Results.NotFound();
    }
    party.Title = updatedParty.Title;
    party.Location = updatedParty.Location;
    party.Time = updatedParty.Time;
    return Results.NoContent();
});

// 删除聚会信息
app.MapDelete("/Party/{id}", (int id) =>
{
    var party = parties.FirstOrDefault(p => p.Id == id);
    if (party is null)
    {
        return Results.NotFound();
    }
    parties.Remove(party);
    deletedIds.Add(id);
    return Results.NoContent();
});

app.Run();
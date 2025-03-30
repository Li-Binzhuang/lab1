using lab1;
using Microsoft.AspNetCore.Http.HttpResults;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// �洢�ۻ���Ϣ���б�
var parties = new List<Party>();
// ��ɾ���ľۻ�Id�б����ڱ���Id�ظ�ʹ��
var deletedIds = new List<int>();

// ��ȡ���оۻ���Ϣ
app.MapGet("/Party/All", () => Results.Ok(parties));

// ��ȡָ��Id�ľۻ���Ϣ
app.MapGet("/Party/{id}", (int id) =>
{
    var party = parties.FirstOrDefault(p => p.Id == id);
    return party is null ? Results.NotFound() : Results.Ok(party);
});

// ����¾ۻ�
app.MapPost("/Party", (Party party) =>
{
    // �����µ�Id������ʹ����ɾ����Id
    int newId = parties.Count == 0 ? 1 : parties.Max(p => p.Id) + 1;
    while (deletedIds.Contains(newId))
    {
        newId++;
    }
    party.Id = newId;
    parties.Add(party);
    return Results.Created($"/Party/{party.Id}", party);
});

// �޸ľۻ���Ϣ
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

// ɾ���ۻ���Ϣ
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
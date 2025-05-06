global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics;
global using System.IdentityModel.Tokens.Jwt;
global using System.Net;
global using System.Net.Mail;
global using System.Security.Claims;
global using System.Text;

global using EFSoft.Authentication.Api.Database;
global using EFSoft.Authentication.Api.Enitites;
global using EFSoft.Authentication.Api.Extentsions;
global using EFSoft.Authentication.Api.Models;
global using EFSoft.Authentication.Api.Services;

global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;

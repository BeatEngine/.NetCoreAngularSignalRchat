const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7033';
// For REST /api/**
// For SignalR /sigR/**
const PROXY_CONFIG = [
  {
    context: ['/api/**'],
    target,
    secure: false
  },
  {
    context: ['/sigR/**'],
    target,
    secure: false,
    ws: true
  }
]

module.exports = PROXY_CONFIG;

import { dev } from './dev';
import { prod } from './prod';

// const env = dev;
const env = dev;

export const environment = {
  serverUrl: env.serverUrl
};
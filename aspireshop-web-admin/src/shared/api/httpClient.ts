import axios from 'axios';

export function createHttpClient(baseURL?: string) {
  return axios.create({
    baseURL,
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
    },
  });
}

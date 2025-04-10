import { API_BASE_PATH } from "@/config";
import { BookMeAPIApi, Configuration } from "../codegen";

const API_CONFIG = new Configuration({
  basePath: API_BASE_PATH,
});

export class ApiService extends BookMeAPIApi {
  constructor() {
    super(API_CONFIG);
  }
}

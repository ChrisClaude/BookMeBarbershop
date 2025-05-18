import { API_BASE_PATH } from "@/config";
import { BookingApi, UserApi,Configuration } from "../codegen";

const API_CONFIG = new Configuration({
  basePath: API_BASE_PATH,
});

export class UserApiWithConfig extends UserApi {
  constructor() {
    super(API_CONFIG);
  }
}

export class BookingApiWithConfig extends BookingApi {
  constructor() {
    super(API_CONFIG);
  }
}

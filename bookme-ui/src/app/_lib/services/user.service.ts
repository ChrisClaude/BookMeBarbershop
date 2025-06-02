import { UserDto } from "../codegen";
import { Result } from "../types/common.types";
import { isStatusCodeSuccess } from "../utils/common.utils";
import { logError } from "../utils/logging.utils";
import { UserApiWithConfig } from "./api.service";

export class UserService {
  protected static userApi = new UserApiWithConfig();

  private static buildHeaders({
    token,
    contentType,
  }: {
    token: string;
    contentType?: string;
  }) {
    const headers = {
      Authorization: `Bearer ${token}`,
      ["content-type"]: contentType || "application/json",
    };

    return headers;
  }

  public static async getUserProfile(): Promise<Result<UserDto | undefined>> {
    try {
      const response = await this.userApi.apiUserMeGetRaw();

      if (!isStatusCodeSuccess(response.raw.status)) {
        const error = await response.raw.json();
        return {
          success: false,
          errors: [error?.toString() || "Error fetching user"],
        };
      }
      const body = await response.raw.json();
      return {
        success: true,
        data: body as UserDto,
      };
    } catch (error) {
      logError(
        error?.toString() || "Error fetching user",
        "GetUserError",
        error
      );
      return {
        success: false,
        errors: [error?.toString() || "Error fetching user"],
      };
    }
  }
}

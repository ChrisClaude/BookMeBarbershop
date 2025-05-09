import { Result } from "../types/common.types";
import { logError } from "../utils/logging.utils";
import { ApiService } from "./api.service";

export class UserService {
  protected static bookMeApi = new ApiService();

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

  public static async getUserProfile({
    token,
  }: any): Promise<Result<string | undefined>> {
    try {
      const headers = this.buildHeaders({ token });
      const response = await this.bookMeApi.apiUsersMeGetRaw({ headers });

      if (response.raw.status !== 200) {
        const error = await response.raw.json();
        return {
          success: false,
          errors: [error?.toString() || "Error fetching user"],
        };
      }
      const body = await response.raw.json();
      return {
        success: true,
        data: body as string,
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

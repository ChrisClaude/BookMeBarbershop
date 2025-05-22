import { describe, expect, it } from "vitest";
import { vi } from "vitest";
import { renderHook } from "@testing-library/react";
import { useAuth } from "../useAuth";

describe("useAuth", () => {
  it("containsRoles should return true if the user has any of the roles", () => {
    // Mock the dependencies before importing the hook
    vi.mock("next-auth/react", () => ({
      useSession: vi.fn(() => ({
        data: { user: { name: "Test User", email: "test@example.com" } },
        status: "authenticated"
      })),
      signIn: vi.fn(),
      signOut: vi.fn()
    }));

    vi.mock("@/_lib/queries", () => ({
      useGetUserProfileQuery: vi.fn(() => ({
        data: {
          roles: [
            { role: { name: "CUSTOMER" } },
            { role: { name: "ADMIN" } }
          ]
        },
        isFetching: false,
        error: null
      }))
    }));

    // Use renderHook to properly call the React hook
    const { result } = renderHook(() => useAuth());

    // Access the containsRoles function from the result
    const { containsRoles } = result.current;

    // Test different scenarios
    expect(containsRoles(["CUSTOMER"])).toBe(true);
    expect(containsRoles(["ADMIN"])).toBe(true);
    expect(containsRoles(["UNKNOWN_ROLE"])).toBe(false);
    expect(containsRoles(["CUSTOMER", "UNKNOWN_ROLE"])).toBe(true);
  });
});
